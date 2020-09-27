using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public partial class ShopMyAddress
    {
        public string ShowAddressName
        {
            get {
                StringBuilder str = new StringBuilder();
                if (string.IsNullOrEmpty(this.ProvinceName) == false)
                    str.Append(this.ProvinceName);
                if (string.IsNullOrEmpty(this.CityName) == false)
                    str.Append(this.CityName);
                if (string.IsNullOrEmpty(this.CountyName) == false)
                    str.Append(this.CountyName);
                str.Append(this.Address);
                return str.ToString();
            }
        }
        public string ShowAddressId
        {
            get
            {
                StringBuilder str = new StringBuilder();
                if (string.IsNullOrEmpty(this.ProvinceName) == false)
                {
                    str.Append(this.ProvId + ",");
                }
                else
                {
                    str.Append(0 + ",");
                }
                if (string.IsNullOrEmpty(this.CityName) == false)
                {
                    str.Append(this.CityID + ",");
                }
                else
                {
                    str.Append(0 + ",");
                }
                if (string.IsNullOrEmpty(this.CountyName) == false)
                {
                    str.Append(this.CountyID + ",");
                }
                else
                {
                    str.Append(0 + ",");
                }
                str.Append(this.Address);
                return str.ToString();
            }
        }
    }
}
