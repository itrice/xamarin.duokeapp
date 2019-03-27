package md5e7e1e2652a818b8e206971d34d14a360;


public class GenericFileProvider
	extends xamarin.essentials.fileProvider
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("JZXY.Duoke.GenericFileProvider, JZXY.Duoke.Android", GenericFileProvider.class, __md_methods);
	}


	public GenericFileProvider ()
	{
		super ();
		if (getClass () == GenericFileProvider.class)
			mono.android.TypeManager.Activate ("JZXY.Duoke.GenericFileProvider, JZXY.Duoke.Android", "", this, new java.lang.Object[] {  });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
