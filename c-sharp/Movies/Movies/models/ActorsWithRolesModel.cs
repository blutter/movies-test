using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.models
{
    public class ActorsWithRolesModel
    {
        private readonly string name;
        private readonly IList<string> roles;

        public ActorsWithRolesModel(string _name, IList<string> _roles)
        {
            name = _name;
            roles = _roles;
        }

        public string Name { get { return name; } }
        public IList<string> Roles { get { return roles; } }
    }
}
