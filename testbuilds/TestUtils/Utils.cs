﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testbuilds.TestUtils {
    class Utils {
        internal static void SetupChoise(ChoisObjekts[] choises) {
            for (int i = 0; i < choises.Length; i++) {
                Console.WriteLine( $"[{i}]: {choises[i]._Name}" );
            }
            Console.WriteLine( "Bitte Wählen" );
            choises[int.Parse( Console.ReadLine() )].Avtivete();
        }
    }
    public class ChoisObjekts {
        public Guid guid { get; private set; }
        public string _Name { get; private set; }

        public ChoisObjekts(string Name) {
            guid = Guid.NewGuid();
            _Name = Name;
        }

        public virtual void Avtivete() {
            Console.WriteLine( "Choised: " + _Name );
        }

        public bool Equals(ChoisObjekts c1, ChoisObjekts c2) {
            if (c1.guid == c2.guid) {
                return true;
            }

            return false;
        }
    }
}
