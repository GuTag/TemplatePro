using Panuon.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.MainClient.Models.Extension
{
    public class TimeProgramOrder : NotifyPropertyChangedBase
    {
		private string _title;

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		private int _time;

		public int Time
		{
			get { return _time; }
			set { _time = value; }
		}

		private string _description;

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		private List<string> _device;

		public List<string> Device
		{
			get { return _device; }
			set { _device = value; }
		}

		private List<string> _controlWord;

		public List<string> ControlWord
		{
			get { return _controlWord; }
			set { _controlWord = value; }
		}





	}
}
