namespace DBMeasurer.Rules
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class MarkItem
    {
        private DateTime _MarkedTime;
        private double _MarkOfDB;
        private string _Name;

        [DataMember]
        public DateTime MarkedTime
        {
            get
            {
                return this._MarkedTime;
            }
            set
            {
                this._MarkedTime = value;
            }
        }

        [DataMember]
        public double MarkOfDB
        {
            get
            {
                return this._MarkOfDB;
            }
            set
            {
                this._MarkOfDB = value;
            }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }
    }
}

