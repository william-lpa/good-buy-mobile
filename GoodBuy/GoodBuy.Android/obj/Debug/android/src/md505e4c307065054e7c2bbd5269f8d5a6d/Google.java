package md505e4c307065054e7c2bbd5269f8d5a6d;


public class Google
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("GoodBuy.Droid.Authentication.Google, GoodBuy.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Google.class, __md_methods);
	}


	public Google () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Google.class)
			mono.android.TypeManager.Activate ("GoodBuy.Droid.Authentication.Google, GoodBuy.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

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
