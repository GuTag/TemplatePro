using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.UI.Helpers
{
    public class Undoable
    {
        List<Step> steps = new List<Step>();
        int index = -1;

        //撤销
        public bool IsUndoEnable => index > -1 && index < steps.Count;

        //重做
        public bool IsRedoEnable => index >= -1 && index < steps.Count - 1;
        public void Undo()
        {
            if(IsUndoEnable)
            {
                steps[index--].Undo();
            }
        }

        public void Redo()
        {
            if (IsRedoEnable)
            {
                steps[++index].Redo();
            }
        }

        public void Clear()
        {
            steps.Clear();
            index = -1;
        }

        public void Add(Action undo, Action redo)
        {
            index++;
            if(index < steps.Count && index > -1)
            {
                steps.RemoveRange(index, steps.Count - index);
            }
            steps.Add(new Step() { Undo = undo, Redo = redo });
        }

        public void Clean()
        {
            steps.Clear();
            index = -1;
        }


        class Step
        {
            //重做
            public Action Redo { get; set; }

            //撤销
            public Action Undo { get; set; }
        }
    }
}
