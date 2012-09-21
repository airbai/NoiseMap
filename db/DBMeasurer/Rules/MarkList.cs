namespace DBMeasurer.Rules
{
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Runtime.Serialization;

    [DataContract]
    public class MarkList
    {
        private List<MarkItem> _MarkList = new List<MarkItem>();
        public const int MaxMarkCount = 10;

        private MarkList()
        {
        }

        public bool AddMark(MarkItem item)
        {
            lock (this.CurrentMarkList)
            {
                if (this.AddMarkInner(item))
                {
                    this.CutLgMaxMarkCount();
                    return true;
                }
                return false;
            }
        }

        private bool AddMarkInner(MarkItem item)
        {
            for (int i = this.CurrentMarkList.get_Count() - 1; i > -1; i--)
            {
                if (item.MarkOfDB <= this.CurrentMarkList.get_Item(i).MarkOfDB)
                {
                    return false;
                }
                if (i == 0)
                {
                    this.CurrentMarkList.Insert(0, item);
                    return true;
                }
                if (item.MarkOfDB <= this.CurrentMarkList.get_Item(i - 1).MarkOfDB)
                {
                    this.CurrentMarkList.Insert(i, item);
                    return true;
                }
            }
            if (this.CurrentMarkList.get_Count() > 10)
            {
                return false;
            }
            this.CurrentMarkList.Insert(this.CurrentMarkList.get_Count(), item);
            return true;
        }

        public void CutLgMaxMarkCount()
        {
            if (this.CurrentMarkList.get_Count() > 10)
            {
                for (int i = this.CurrentMarkList.get_Count() - 1; i > 9; i--)
                {
                    this.CurrentMarkList.RemoveAt(i);
                }
            }
        }

        public bool IsMarked(double mark)
        {
            lock (this.CurrentMarkList)
            {
                if (this.CurrentMarkList.get_Count() >= 10)
                {
                    return ((this.CurrentMarkList.get_Item(this.CurrentMarkList.get_Count() - 1).MarkOfDB > mark) ? 0 : 1);
                }
                return true;
            }
        }

        public static MarkList Load()
        {
            MarkList list;
            if (IsolatedStorageSettings.get_ApplicationSettings().TryGetValue<MarkList>("MarkList", ref list))
            {
                return list;
            }
            return new MarkList();
        }

        public void Save()
        {
            IsolatedStorageSettings.get_ApplicationSettings().set_Item("MarkList", this);
            IsolatedStorageSettings.get_ApplicationSettings().Save();
        }

        [DataMember]
        public List<MarkItem> CurrentMarkList
        {
            get
            {
                return this._MarkList;
            }
            set
            {
                this._MarkList = value;
            }
        }
    }
}

