
using System;
using System.Ai.Modle;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace System.Ai {
    public class AIMasterInt64 {
        public MInt64[] modleList;
        private DateTime ts;

        public AIMasterInt64(MInt64[] modles) {
            ts = DateTime.Now;
            modleList = modles;
        }
        private static MInt64 init(MInt64 m) {
            m.init = true;
            return m;
        }
        public void trainloop(MInt64 m, long oSetMin, long oSetMax, bool output = true) {
            ts = DateTime.Now;
            while (!Train( m, oSetMin, oSetMax, output )) {

            }
        }

        public bool Train(MInt64 m, long oSetMin, long oSetMax, bool output = true) {
            if (m.doubs) {
                if (m.w == 0) {
                    if (m.addition) {
                        m.w = oSetMin;
                    } else {
                        m.w = -oSetMin;
                    }
                }

                var prev = (long) m.Variable;

                m.Longs[m.currentIndex] += m.w;
                m.algo.OnMainInt64( m );

                if (m.Variable > m.Ziehl && m.Variable < prev) { m.addition = false; } else if (( m.Variable < m.Ziehl && m.Variable < prev ) || ( m.Variable > m.Ziehl && m.Variable > prev )) {
                    m.addition = !m.addition;
                } else if (m.Variable < m.Ziehl && m.Variable > prev) { m.addition = true; }
                m.w = 0;

                if (m.Variable < m.Ziehl + m.tolleranz && m.Variable > m.Ziehl - m.tolleranz) {
                    Finished( m );
                    return true;
                }
                if (prev == m.Variable) {
                    m.w += m.addition ? oSetMax : -oSetMin;
                }
                if (output)
                    Console.WriteLine( $"ONext:{m.w}, prew:{prev}, post:{m.Variable}, tolleranz:{m.tolleranz}" );
                m.round += 1;
            }
            if (m.round == 10) {
                m.currentIndex += 1;
                if (m.currentIndex >= m.Longs.Length) {
                    m.currentIndex = 0;
                }
            }
            if (m.round > 20) {
                m.round = 0;
            }

            return false;
        }

        private void Finished(MInt64 m) {
            Console.Write( "Finished in " + ( DateTime.Now - ts ) + $" Final Vaule:{m.Variable}, FInal Longs:" );
            for (var i = 0; i < m.length; i++) {
                Console.Write( $"doubs[{i}]: {m.Doubles[i]}, " );
            }
            Console.WriteLine();
        }
    }
    public class AIMasterDouble {
        public MDouble[] modleList;
        private DateTime ts;

        public AIMasterDouble(MDouble[] modles) {
            ts = DateTime.Now;
            modleList = modles;
        }
        private static MDouble init(MDouble m) {
            m.init = true;
            return m;
        }
        public void trainloop(MDouble m, double oSetMin, double oSetMax, bool output = true) {
            ts = DateTime.Now;
            while (!Train( m, oSetMin, oSetMax, output )) {

            }
        }

        public bool Train(MDouble m, double oSetMin, double oSetMax, bool output = true) {
            if (m.doubs) {
                if (m.w == 0) {
                    if (m.addition) {
                        m.w = oSetMin;
                    } else {
                        m.w = -oSetMin;
                    }
                }

                var prev = m.Variable;

                m.Doubles[m.currentIndex] += m.w;
                m.algo( m, output );

                if (m.Variable > m.Ziehl && m.Variable < prev) { m.addition = false; } else if (( m.Variable < m.Ziehl && m.Variable < prev ) || ( m.Variable > m.Ziehl && m.Variable > prev )) {
                    m.addition = !m.addition;
                } else if (m.Variable < m.Ziehl && m.Variable > prev) { m.addition = true; }
                m.w = 0;

                if (m.Variable < m.Ziehl + m.tolleranz && m.Variable > m.Ziehl - m.tolleranz) {
                    Finished( m );
                    return true;
                }
                if (prev == m.Variable) {
                    m.w += m.addition ? oSetMax : -oSetMin;
                }
                if (output)
                    Console.WriteLine( $"ONext:{m.w}, prew:{prev}, post:{m.Variable}, tolleranz:{m.tolleranz}" );
                m.round += 1;
            }
            if (m.round == 10) {
                m.currentIndex += 1;
                if (m.currentIndex >= m.Doubles.Length) {
                    m.currentIndex = 0;
                }
            }
            if (m.round > 20) {
                m.round = 0;
            }

            return false;
        }

        private void Finished(MDouble m) {
            Console.Write( "Finished in " + ( DateTime.Now - ts ) + $" Final Vaule:{m.Variable}, FInal Longs:" );
            for (var i = 0; i < m.length; i++) {
                Console.Write( $"doubs[{i}]: {m.Doubles[i]}, " );
            }
            Console.WriteLine( "      " );
        }
    }
}

namespace System.Ai.Modle {

    public class MInt64 : Base {
        public long w = 0;
        public long length { get; private set; }

        private long _Vaule = -1;
        public long Vaule { get { if (!isOnVaule) { return _Vaule; } else { return -1; } } private set => _Vaule = value; }
        public bool isOnVaule { get; private set; }

        public int currentIndex { get; set; }
        public Algos algo;
        public bool addition = true;
        public int round = 0;

        public long tolleranz = 0;

        public MInt64(int Ziehlwert, int startwert, long[] _long, Algos algos, long _tolleranz = 0) : base( Ziehlwert, startwert, null, null, null, _long ) {
            tolleranz = _tolleranz;
            algo = algos;

            length = _long.Length;
            if (length == 1) {
                isOnVaule = true;
                Vaule = _long[0];
            }
            currentIndex = 0;

        }
    }
    public class MDouble : Base {
        public Guid Guid = Guid.NewGuid();
        public double w = 0;
        public int length { get; private set; }

        private double _Vaule = -1;
        public double Vaule { get { if (!isOnVaule) { return _Vaule; } else { return -1; } } private set => _Vaule = value; }
        public bool isOnVaule { get; private set; }

        public int currentIndex { get; set; }
        public Action<MDouble, bool> algo;
        public bool addition = true;
        public int round = 0;

        public double tolleranz = 0;

        public MDouble(int Ziehlwert, int startwert, double[] _doubs, Action<MDouble, bool> algos, double _tolleranz = 0) : base( Ziehlwert, startwert, null, _doubs, null, null ) {
            tolleranz = _tolleranz;
            algo = algos;

            length = _doubs.Length;
            if (length == 1) {
                isOnVaule = true;
                Vaule = _doubs[0];
            }
            currentIndex = 0;

        }
    }
    public class Algos {
        public virtual void OnMainInt64(MInt64 m) { }

        public virtual void OnMainDouble(MDouble m, bool ausgabe = false) { }
    }
    public class Base {
        public bool init = false;

        public int[] Integers;
        private double[] Doubles_;

        public double[] Doubles {
            get => Doubles_;
            set =>
                //Console.WriteLine("             set             ");
                Doubles_ = value;
        }

        public string[] Strings;
        public long[] Longs;

        public bool ints = true;
        public bool strs = true;
        public bool doubs = true;
        public bool longs = true;

        private double _Variable;
        public double Variable {
            get => _Variable;
            set => _Variable = value;
        }

        public int Ziehl { get; private set; }

        public Base(int Ziehlwert, int startwert, int[] _ints, double[] _doub, string[] _str, long[] _long) {
            Variable = startwert;
            Ziehl = Ziehlwert;

            if (_ints != null) {
                Integers = _ints;
            } else {
                ints = false;
            }

            if (_doub != null) {
                Doubles = _doub;
            } else {
                doubs = false;
            }

            if (_str != null) {
                Strings = _str;
            } else {
                strs = false;
            }

            if (_long != null) {
                Longs = _long;
            } else {
                longs = false;
            }
        }
    }
}
