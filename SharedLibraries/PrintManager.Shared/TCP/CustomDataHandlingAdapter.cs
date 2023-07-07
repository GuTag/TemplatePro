using System;
using System.Linq;
using System.Text;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace PrintManager.Shared.TCP
{

    public class CustomDataHandlingAdapter : CustomUnfixedHeaderDataHandlingAdapter<RequestInfo>
    {

        public override bool CanSendRequestInfo => true;

        protected override RequestInfo GetInstance()
        {
            return new RequestInfo();
        }

        //protected override void PreviewSend(IRequestInfo requestInfo, bool isAsync)
        //{
        //}
    }



    public class RequestInfo : IUnfixedHeaderRequestInfo
    {
        private byte[] terminatorCode = Encoding.UTF8.GetBytes("\r\n");
        public int BodyLength { get; private set; }
        public string ID { get; set; }
        public string Data { get; set; }

        public RequestInfo()
        {

        }
        public RequestInfo(string ID, string Data)
        {
            this.ID = ID;
            this.Data = Data;
        }

        public bool OnParsingHeader(ByteBlock byteBlock)
        {
            int index = byteBlock.Buffer.IndexOfFirst(byteBlock.Pos, Convert.ToInt32(byteBlock.Length), terminatorCode);
            if (index > 0)
            {
                //int headerLength = index - 0;
                int headerLength = index - byteBlock.Pos;
                this.ReadHeaders(byteBlock.Buffer, index + byteBlock.Pos, headerLength);
                //byteBlock.Pos += index + terminatorCode.Length + 1;
                byteBlock.Pos += headerLength + terminatorCode.Length + 2 + 1;
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool OnParsingBody(byte[] body)
        {
            if (body.Length != BodyLength) return false;

            var data = body.Skip(0).Take(this.BodyLength - 2).ToArray();
            Data = Encoding.UTF8.GetString(data);
            //CRCÐ£Ñé
            var crc = body.Skip(this.BodyLength - 2).Take(this.BodyLength).ToArray();
            var crc_new = Crc.Crc16(data);
            if (Enumerable.SequenceEqual(crc, crc_new))
            {
                return true;
            }
            return false;
        }

        private void ReadHeaders(byte[] buffer, int offset, int length)
        {
            ID = Encoding.UTF8.GetString(buffer, 0, length - 1);
            BodyLength = (int)TouchSocketBitConverter.BigEndian.ToInt32(buffer, offset + terminatorCode.Length - 1);

        }
        //public bool OnParsingHeader(byte[] header)
        //{
        //    if(header.Length != 14) return false;
        //    ID = Encoding.UTF8.GetString(header, 0, 10);
        //    //Type = Encoding.UTF8.GetString(header, 0, 10);
        //    BodyLength = (int)TouchSocketBitConverter.BigEndian.ToInt32(header, 10);
        //    return true;
        //}

        public void Build(ByteBlock byteBlock)
        {
            byteBlock.Write(Encoding.UTF8.GetBytes(ID));
            byteBlock.Write(terminatorCode);

            if(Data == null || Data.Length == 0)
            {
                Data = "{\"status\":\"error\"}";
            }
            var body = Encoding.UTF8.GetBytes(Data);
            BodyLength = body.Length + 2;
            byteBlock.Write(TouchSocketBitConverter.BigEndian.GetBytes(BodyLength));
            byteBlock.Write(body);
            byteBlock.Write(Crc.Crc16(body));
        }

        public byte[] BuildAsBytes()
        {
            using(ByteBlock byteBlock = new ByteBlock())
            {
                this.Build(byteBlock);
                return byteBlock.ToArray();
            }
        }

        public override string ToString()
        {
            return $"ID={ID}, DataLength={(Data == null ? 0 : Data.Length)}";
        }

       
    }
}
