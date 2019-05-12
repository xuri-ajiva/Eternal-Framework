using Eternal.Ai.Modle;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Documents;

namespace Eternal.Ai {
    internal class AiMasterInt64 : EternalFramework.EternalMain {
        public MInt64[] ModleList;
        private DateTime _ts;

        public AiMasterInt64(MInt64[] modles) {
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
                    }
                    else {
                        m.W = -oSetMin;
                    }
                }

                var prev = (long) m.Variable;

                m.Longs[m.CurrentIndex] += m.W;
                m.Algo( m, output );

                if (m.Variable > m.Ziehl && m.Variable < prev) { m.Addition = false; }
                else if (( m.Variable < m.Ziehl && m.Variable < prev ) || ( m.Variable > m.Ziehl && m.Variable > prev )) {
                    m.Addition = !m.Addition;
                }
                else if (m.Variable < m.Ziehl && m.Variable > prev) { m.Addition = true; }
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
    internal class AiMasterDouble : EternalFramework.EternalMain {
        public MDouble[] ModleList;
        private DateTime _ts;

        public AiMasterDouble(MDouble[] modles) {
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
                    }
                    else {
                        m.W = -oSetMin;
                    }
                }

                var prev = m.Variable;

                m.Doubles[m.CurrentIndex] += m.W;
                m.Algo( m, output );

                if (m.Variable > m.Ziehl && m.Variable < prev) { m.Addition = false; }
                else if (( m.Variable < m.Ziehl && m.Variable < prev ) || ( m.Variable > m.Ziehl && m.Variable > prev )) {
                    m.Addition = !m.Addition;
                }
                else if (m.Variable < m.Ziehl && m.Variable > prev) { m.Addition = true; }
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

    public class AiMasterBase : EternalFramework.EternalMain {
        private readonly ModuleBase _curentBase;
        private DateTime _ts;

        private List<stats> _stats = new List<stats>();

        public void Trainloop(bool output = true) {
            _ts = DateTime.Now;
            while (!Train( output )) {

            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public bool Train(bool output = true) {
            var @base = _curentBase;

            var min = @base.MinOfset;
            var max = @base.MaxOfset;

            var Variable = @base.Variable;
            var Thaget = @base.Thaget;
            var Tolleranz = @base.Tolleranz;
            try {

                if (@base.NextTry == 0) {
                    if (@base.Addition) @base.NextTry = min;
                    else @base.NextTry = -min;
                }

                {
                    var prev = Variable;

                    @base.ToChange[@base.CurrentIndex] += @base.NextTry;

                    @base.Action( _curentBase, output );

                    Variable = @base.Variable;

                    if (Variable > Thaget && Variable < prev) {
                        @base.Addition = false;
                    }
                    else if (( Variable < Thaget && Variable < prev ) || ( Variable > Thaget && Variable > prev )) {
                        @base.Addition = !@base.Addition;
                    }
                    else if (Variable < Thaget && Variable > prev) {
                        @base.Addition = true;
                    }

                    @base.NextTry = 0;

                    if (Variable < Thaget + Tolleranz && Variable > Thaget - Tolleranz) {
                        Finished( _curentBase );
                        return true;
                    }

                    if (prev == Variable) {
                        @base.NextTry += @base.Addition ? max : -max;
                    }

                    if (output)
                        Console.WriteLine($"Next:{@base.NextTry}, prew:{prev}, post:{Variable}, tolleranz:{Tolleranz}" );
                    @base.Round += 1;
                }
                if (@base.Round % 20 == 10) {
                    @base.CurrentIndex += 1;
                    if (@base.CurrentIndex >= @base.ToChange.Length) {
                        @base.CurrentIndex = 0;
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine( e.Message );
            }

            return false;
        }

        private void Finished(ModuleBase @base) {
            var ts = DateTime.Now - _ts;
            Console.WriteLine( $"Finished in {ts}, Final Vaule:{@base.Variable}, Rounds:{@base.Round}, ToChange:" );
            for (var i = 0; i < @base.ToChange.Length; i++) {
                Console.WriteLine( $"[{i}]: {@base.ToChange[i]}." );
            }
            Console.WriteLine( "      " );
            var s = new stats( @base.Round, ts, @base.Variable );
            _stats.Add( s );
        }

        public AiMasterBase(ModuleBase curentBase) {
            _curentBase = curentBase;
        }

        public ModuleBase CurentBase => _curentBase;

        public List<stats> Stats => _stats;
    }

    public class stats {
        private int Round;
        private TimeSpan Time;
        private double finalvaule;

        public stats(int round, TimeSpan time, double finalvaule) {
            Round = round;
            Time = time;
            this.finalvaule = finalvaule;
        }

        public double Finalvaule => finalvaule;

        public int Round1 => Round;

        public TimeSpan Time1 => Time;
    }
}

namespace Eternal.Ai.Modle {

    public class ModuleBase : EternalFramework.EternalMain {
        private bool Init;

        private dynamic _variable;
        private readonly dynamic _thaget;
        private readonly dynamic _minOfset;
        private readonly dynamic _maxOfset;

        private dynamic[] _toChange;
        private int _currentIndex;
        private int _round;
        private int _tolleranz;

        private Action<ModuleBase, bool> _action;

        private bool _addition = true;
        private dynamic _nextTry;

        public ModuleBase(dynamic variable, dynamic thaget, dynamic minOfset, dynamic maxOfset, dynamic[] Tochange, Action<ModuleBase, bool> action, int tolleranz) : base() {
            Init = true;
            _variable = variable;
            _thaget = thaget;
            _minOfset = minOfset;
            _maxOfset = maxOfset;
            _toChange = Tochange;
            _action = action;
            _tolleranz = tolleranz;
            _currentIndex = 0;
            _nextTry = 0;
            _round = 0;
        }

        public int Tolleranz {
            get => _tolleranz;
            set => _tolleranz = value;
        }

        public int Round {
            get => _round;
            set => _round = value;
        }

        public Action<ModuleBase, bool> Action {
            get => _action;
            set => _action = value;
        }

        public int CurrentIndex {
            get => _currentIndex;
            set => _currentIndex = value;
        }

        public dynamic Variable {
            get => _variable;
            set => _variable = value;
        }

        public dynamic NextTry {
            get => _nextTry;
            set => _nextTry = value;
        }

        public dynamic[] ToChange {
            get => _toChange;
            set => _toChange = value;
        }

        public bool Addition {
            get => _addition;
            set => _addition = value;
        }

        public dynamic MaxOfset => _maxOfset;

        public dynamic MinOfset => _minOfset;

        public dynamic Thaget => _thaget;
    }


    internal class MInt64 : Base {
        public long W;
        public long Length { get; private set; }

        private long _vaule = -1;
        public long Vaule {
            get {
                if (!IsOnVaule) { return _vaule; }

                return -1;
            }
            private set => _vaule = value;
        }
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
    internal class MDouble : Base {
        public double W;
        public int Length { get; private set; }

        private double _vaule = -1;
        public double Vaule {
            get {
                if (!IsOnVaule) { return _vaule; }

                return -1;
            }
            private set => _vaule = value;
        }
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
    internal class Algos : EternalFramework.EternalMain {
        public virtual void OnMainInt64(MInt64 m) { }

        public virtual void OnMainDouble(MDouble m, bool ausgabe = false) { }
    }
    internal class Base : EternalFramework.EternalMain {
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

        public Base(int ziehlwert, int startwert, int[] _ints, double[] _doub, string[] _str, long[] _long) {
            Variable = startwert;
            Ziehl = ziehlwert;

            if (_ints != null) {
                Integers = _ints;
            }
            else {
                ints = false;
            }

            if (_doub != null) {
                Doubles = _doub;
            }
            else {
                doubs = false;
            }

            if (_str != null) {
                Strings = _str;
            }
            else {
                strs = false;
            }

            if (_long != null) {
                Longs = _long;
            }
            else {
                longs = false;
            }
        }

    }
}
