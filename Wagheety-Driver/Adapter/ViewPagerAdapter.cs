using System.Collections.Generic;
using Android.Support.V4.App;

namespace Wagheety_Driver.Adapter
{
    public class ViewPagerAdapter : FragmentPagerAdapter
    {
       public List<Fragment> fragments { get; set; }
       public List<string> fragmentNames { get; set; }

        public ViewPagerAdapter(FragmentManager fragmentManager) : base(fragmentManager)
        {
            fragments = new List<Fragment>();
            fragmentNames = new List<string>();
        }

        public void AddFragemts(Fragment fragment , string name)
        {
            fragments.Add(fragment);
            fragmentNames.Add(name);
        }
        public override int Count
        {
            get
            {
                return fragments.Count;
            }
        }

        public override Fragment GetItem(int position)
        {
            return fragments[position];
        }
    }
}