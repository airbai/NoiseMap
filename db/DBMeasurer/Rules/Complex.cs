namespace DBMeasurer.Rules
{
    using System;

    public class Complex
    {
        private double image;
        private double real;

        public Complex() : this(0.0, 0.0)
        {
        }

        public Complex(double real) : this(real, 0.0)
        {
        }

        public Complex(double real, double image)
        {
            this.real = real;
            this.image = image;
        }

        public static Complex operator +(Complex c1, Complex c2)
        {
            return new Complex(c1.real + c2.real, c1.image + c2.image);
        }

        public static Complex operator *(Complex c1, Complex c2)
        {
            return new Complex((c1.real * c2.real) - (c1.image * c2.image), (c1.image * c2.real) + (c1.real * c2.image));
        }

        public static Complex operator -(Complex c1, Complex c2)
        {
            return new Complex(c1.real - c2.real, c1.image - c2.image);
        }

        public double ToModul()
        {
            return Math.Sqrt((this.real * this.real) + (this.image * this.image));
        }

        public override string ToString()
        {
            if ((this.Real == 0.0) && (this.Image == 0.0))
            {
                return string.Format("{0}", 0);
            }
            if (((this.Real == 0.0) && (this.Image != 1.0)) && (this.Image != -1.0))
            {
                return string.Format("{0} i", this.Image);
            }
            if (this.Image == 0.0)
            {
                return string.Format("{0}", this.Real);
            }
            if (this.Image == 1.0)
            {
                return string.Format("i", new object[0]);
            }
            if (this.Image == -1.0)
            {
                return string.Format("- i", new object[0]);
            }
            if (this.Image < 0.0)
            {
                return string.Format("{0} - {1} i", this.Real, -this.Image);
            }
            return string.Format("{0} + {1} i", this.Real, this.Image);
        }

        public double Image
        {
            get
            {
                return this.image;
            }
            set
            {
                this.image = value;
            }
        }

        public double Real
        {
            get
            {
                return this.real;
            }
            set
            {
                this.real = value;
            }
        }
    }
}

