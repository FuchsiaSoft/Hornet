using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hornet.Model
{
    partial class HashGroup
    {
        public int MD5Count { get { return GetMD5Count(); } }

        private int GetMD5Count()
        {
            try
            {
                using (HornetModelContainer db = new HornetModelContainer())
                {
                    return db.HashEntries.OfType<MD5>().Count(c => c.HashGroupId == Id);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int SHA1Count { get { return GetSHA1Count(); } }

        private int GetSHA1Count()
        {
            try
            {
                using (HornetModelContainer db = new HornetModelContainer())
                {
                    return db.HashEntries.OfType<SHA1>().Count(c => c.HashGroupId == Id);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int SHA256Count { get { return GetSHA256Count(); } }

        private int GetSHA256Count()
        {
            try
            {
                using (HornetModelContainer db = new HornetModelContainer())
                {
                    return db.HashEntries.OfType<SHA256>().Count(c => c.HashGroupId == Id);
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
