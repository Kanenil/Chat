using System;
using System.Collections.Generic;
using System.Text;

namespace ClientWPF.Stores
{
    public class PeopleStore
    {
        public event Action<string> PersonAdded;

        public void AddPerson(string name)
        {
            PersonAdded?.Invoke(name);
        }
    }
}
