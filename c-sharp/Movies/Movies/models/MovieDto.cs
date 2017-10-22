using System;
using System.Collections.Generic;
using System.Text;

namespace Movies.models
{
    public class MovieDto
    {
        public string Name { get; set; }
        public IList<RoleDto> Roles { get; set; }
    }
}
