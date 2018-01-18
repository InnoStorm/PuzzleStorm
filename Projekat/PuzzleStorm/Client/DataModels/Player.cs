using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client {
    public sealed class Player {

        private static Player instance = null;

        public int Id { get; set; } = -1;

        private Player() {
        }

        public static Player Instance {
            get {
                if (instance == null) {
                    instance = new Player();
                }

                return instance;
            }
        }
    }
}
