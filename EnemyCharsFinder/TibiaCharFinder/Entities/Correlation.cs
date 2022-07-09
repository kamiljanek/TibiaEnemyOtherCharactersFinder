using System;
<<<<<<< HEAD
<<<<<<< HEAD
=======
using System.Collections.Generic;
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd
=======
using System.Collections.Generic;
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd

namespace TibiaCharFinder.Entities
{
    public class Correlation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public DateTime LogInOrLogOutDateTime { get; set; }
<<<<<<< HEAD
<<<<<<< HEAD
        public int PossibleCharacterId { get; set; }
=======
        public string PossibleOtherCharacters { get; set; }
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd
=======
        public string PossibleOtherCharacters { get; set; }
>>>>>>> 7adfd3e7255e39674243874a51dd79850ec583bd
        public virtual Character Character { get; set; }
    }
}
