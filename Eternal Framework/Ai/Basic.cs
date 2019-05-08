using System;
using Eternal.Ai.Modle;

namespace Eternal.Ai {
    public class AiMasterInt64 : EternalFramework.Eternal {
        public MInt64[] ModleList;
        private DateTime _ts;

        public AiMasterInt64(MInt64[] modles)
        {
            _ts = DateTime.Now;
            ModleList = modles;
        }
        private static MInt64 Init(MInt64 m) {
            m.init = true;
            return m;
        }
        public void Trainloop(MInt64 m, long oSetMin, long oSetMax, bool output = true) {
            _ts = DateTime.Now;
            while (!Train( m, oSetMin, oSetMax, output )) {

            }
        }

        public bool Train(MInt64 m, long oSetMin, long oSetMax, bool output = true) {
            if (m.doubs) {
                if (m.W == 0) {
                    if (m.Addition) {
                        m.W = oSetMin;
                    } else {
                        m.W = -oSetMin;
                    }
                }

                var prev = (long) m.Variable;

                m.Longs[m.CurrentIndex] += m.W;
                m.Algo( m, output );

                if (m.Variable > m.Ziehl && m.Variable < prev) { m.Addition = false; } else if (( m.Variable < m.Ziehl && m.Variable < prev ) || ( m.Variable > m.Ziehl && m.Variable > prev )) {
                    m.Addition = !m.Addition;
                } else if (m.Variable < m.Ziehl && m.Variable > prev) { m.Addition = true; }
                m.W = 0;

                if (m.Variable < m.Ziehl + m.Tolleranz && m.Variable > m.Ziehl - m.Tolleranz) {
                    Finished( m );
                    return true;
                }
                if (prev == m.Variable) {
                    m.W += m.Addition ? oSetMax : -oSetMin;
                }
                if (output)
                    Console.WriteLine( $"ONext:{m.W}, prew:{prev}, post:{m.Variable}, tolleranz:{m.Tolleranz}" );
                m.Round += 1;
            }
            if (m.Round == 10) {
                m.CurrentIndex += 1;
                if (m.CurrentIndex >= m.Longs.Length) {
                    m.CurrentIndex = 0;
                }
            }
            if (m.Round > 20) {
                m.Round = 0;
            }

            return false;
        }

        private void Finished(MInt64 m) {
            Console.Write( "Finished in " + ( DateTime.Now - _ts ) + $" Final Vaule:{m.Variable}, FInal Longs:" );
            for (var i = 0; i < m.Length; i++) {
                Console.Write( $"doubs[{i}]: {m.Doubles[i]}, " );
            }
            Console.WriteLine();
        }
    }
    public class AiMasterDouble : EternalFramework.Eternal {
        public MDouble[] ModleList;
        private DateTime _ts;

        public AiMasterDouble(MDouble[] modles)
        {
            _ts = DateTime.Now;
            ModleList = modles;
        }
        private static MDouble Init(MDouble m) {
            m.init = true;
            return m;
        }
        public void Trainloop(MDouble m, double oSetMin, double oSetMax, bool output = true) {
            _ts = DateTime.Now;
            while (!Train( m, oSetMin, oSetMax, output )) {

            }
        }

        public bool Train(MDouble m, double oSetMin, double oSetMax, bool output = true) {
            if (m.doubs) {
                if (m.W == 0) {
                    if (m.Addition) {
                        m.W = oSetMin;
                    } else {
                        m.W = -oSetMin;
                    }
                }

                var prev = m.Variable;

                m.Doubles[m.CurrentIndex] += m.W;
                m.Algo( m, output );

                if (m.Variable > m.Ziehl && m.Variable < prev) { m.Addition = false; } else if (( m.Variable < m.Ziehl && m.Variable < prev ) || ( m.Variable > m.Ziehl && m.Variable > prev )) {
                    m.Addition = !m.Addition;
                } else if (m.Variable < m.Ziehl && m.Variable > prev) { m.Addition = true; }
                m.W = 0;

                if (m.Variable < m.Ziehl + m.Tolleranz && m.Variable > m.Ziehl - m.Tolleranz) {
                    Finished( m );
                    return true;
                }
                if (prev == m.Variable) {
                    m.W += m.Addition ? oSetMax : -oSetMin;
                }
                if (output)
                    Console.WriteLine( $"ONext:{m.W}, prew:{prev}, post:{m.Variable}, tolleranz:{m.Tolleranz}" );
                m.Round += 1;
            }
            if (m.Round == 10) {
                m.CurrentIndex += 1;
                if (m.CurrentIndex >= m.Doubles.Length) {
                    m.CurrentIndex = 0;
                }
            }
            if (m.Round > 20) {
                m.Round = 0;
            }

            return false;
        }

        private void Finished(MDouble m) {
            Console.Write( "Finished in " + ( DateTime.Now - _ts ) + $" Final Vaule:{m.Variable}, FInal Longs:" );
            for (var i = 0; i < m.Length; i++) {
                Console.Write( $"doubs[{i}]: {m.Doubles[i]}, " );
            }
            Console.WriteLine( "      " );
        }
    }
}

namespace Eternal.Ai.Modle {

    public class MInt64 : Base {
        public long W;
        public long Length { get; private set; }

        private long _vaule = -1;
        public long Vaule { get
        {
            if (!IsOnVaule) { return _vaule; }

            return -1;
        } private set => _vaule = value; }
        public bool IsOnVaule { get; private set; }

        public int CurrentIndex { get; set; }
        public Action<MInt64, bool> Algo;
        public bool Addition = true;
        public int Round;

        public long Tolleranz;

        public MInt64(int ziehlwert, int startwert, long[] _long, Action<MInt64, bool> algos, long tolleranz = 0) : base( ziehlwert, startwert, null, null, null, _long ) {
            Tolleranz = tolleranz;
            Algo = algos;

            Length = _long.Length;
            if (Length == 1) {
                IsOnVaule = true;
                Vaule = _long[0];
            }
            CurrentIndex = 0;

        }
    }
    public class MDouble : Base {
        public double W;
        public int Length { get; private set; }

        private double _vaule = -1;
        public double Vaule { get
        {
            if (!IsOnVaule) { return _vaule; }

            return -1;
        } private set => _vaule = value; }
        public bool IsOnVaule { get; private set; }

        public int CurrentIndex { get; set; }
        public Action<MDouble, bool> Algo;
        public bool Addition = true;
        public int Round;

        public double Tolleranz;

        public MDouble(int ziehlwert, int startwert, double[] doubs, Action<MDouble, bool> algos, double tolleranz = 0) : base( ziehlwert, startwert, null, doubs, null, null ) {
            Tolleranz = tolleranz;
            Algo = algos;

            Length = doubs.Length;
            if (Length == 1) {
                IsOnVaule = true;
                Vaule = doubs[0];
            }
            CurrentIndex = 0;

        }
    }
    internal class Algos : EternalFramework.Eternal {
        public virtual void OnMainInt64(MInt64 m) { }

        public virtual void OnMainDouble(MDouble m, bool ausgabe = false) { }
    }
    public class Base : EternalFramework.Eternal {
        public bool init;

        public int[] Integers;
        private double[] _doubles;

        public double[] Doubles {
            get => _doubles;
            set =>
                //Console.WriteLine("             set             ");
                _doubles = value;
        }

        public string[] Strings;
        public long[] Longs;

        public bool ints = true;
        public bool strs = true;
        public bool doubs = true;
        public bool longs = true;

        private double _variable;
        public double Variable {
            get => _variable;
            set => _variable = value;
        }

        public int Ziehl { get; private set; }

        public Base(int ziehlwert, int startwert, int[] _ints, double[] _doub, string[] _str, long[] _long)
        {
            Variable = startwert;
            Ziehl = ziehlwert;

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
