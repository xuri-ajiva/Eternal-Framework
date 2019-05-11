using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace overlaytests {
    class complex {

        public double a;
        public double b;

        public void Square() {
            double tmp = ( a * a ) - ( b * b );

            b = 2.0 * a * b;
            a = tmp;
        }

        public double Magnitude() {
            return Math.Sqrt( ( a * a ) + ( b * b ) );
        }

        public void Add(complex c) {
            a += c.a;
            b += c.b;
        }

        public complex(double a, double b) {
            this.a = a;
            this.b = b;
        }
    }
}
