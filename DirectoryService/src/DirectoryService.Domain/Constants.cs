using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectoryService.Domain
{
    public static class Constants
    {
        public const int MIN_NAME_LENGTH = 3;

        public const int MAX_DEPARTMENT_IDENTIFIER_LENGTH = 150;
        public const int MAX_DEPARTMENT_NAME_LENGTH = 150;
        public const int MAX_DEPARTMENT_PATH_LENGTH = 300;

        public const int MAX_LOCATION_NAME_LENGTH = 120;
        public const int MAX_LOCATION_ADDRESS_LENGTH = 200;
        public const int MAX_LOCATION_TIMEZONE_LENGTH = 100;

        public const int MAX_POSITION_NAME_LENGTH = 100;
        public const int MAX_POSITION_DESCRIPTION_LENGTH = 1000;
    }
}
