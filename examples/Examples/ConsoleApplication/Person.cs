using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Person
    {
        #region Members
        private string _name;
        private string _surname;
        private byte _age;
        #endregion

        #region Properties
        public string Name { get => _name; }
        public string Surname { get => _surname; }
        public byte Age { get => _age; }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Person()
        {

        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The Name</param>
        /// <param name="surname">The Surname</param>
        /// <param name="age">The age</param>
        public Person(string name, string surname, byte age)
        {

            _name = name;
            _surname = surname;
            _age = age;

        }


    }
}
