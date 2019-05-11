using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eternal.MathE {
    public struct Complex {

        public double real;
        public double imaginary;

        public void Square() {
            double tmp = ( real * real ) - ( imaginary * imaginary );

            imaginary = 2.0 * real * imaginary;
            real = tmp;
        }

        public double Magnitude() {
            return Math.Sqrt( ( real * real ) + ( imaginary * imaginary ) );
        }

        public void Add(Complex c) {
            real += c.real;
            imaginary += c.imaginary;
        }
        public void Subrtact(Complex c) {
            real -= c.real;
            imaginary -= c.imaginary;
        }

        public Complex(double real, double imaginary) {
            this.real = real;
            this.imaginary = imaginary;
        }

        #region operators

        public static Complex operator +(Complex one, Complex two) {
            return new Complex( one.real + two.real, one.imaginary + two.imaginary );
        }
        public static Complex operator -(Complex one, Complex two) {
            return new Complex( one.real - two.real, one.imaginary - two.imaginary );
        }
        public static Complex operator *(Complex one, Complex two) {
            return new Complex( one.real * two.real, one.imaginary * two.imaginary );
        }
        public static Complex operator /(Complex one, Complex two) {
            return new Complex( one.real / two.real, one.imaginary / two.imaginary );
        } 
        #endregion
    }
}
