//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: ProtoRoute.proto
// Note: requires additional types generated from: ProtoBasis.proto
namespace comrt.comnet
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PRServer")]
  public partial class PRServer : global::ProtoBuf.IExtensible
  {
    public PRServer() {}
    
    private int _id = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int id
    {
      get { return _id; }
      set { _id = value; }
    }
    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private string _ip = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"ip", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ip
    {
      get { return _ip; }
      set { _ip = value; }
    }
    private int _port = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"port", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int port
    {
      get { return _port; }
      set { _port = value; }
    }
    private string _url = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"url", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string url
    {
      get { return _url; }
      set { _url = value; }
    }
    private comrt.comnet.eServerType _type = comrt.comnet.eServerType.UNKNOWN20;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(comrt.comnet.eServerType.UNKNOWN20)]
    public comrt.comnet.eServerType type
    {
      get { return _type; }
      set { _type = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PRServerList")]
  public partial class PRServerList : global::ProtoBuf.IExtensible
  {
    public PRServerList() {}
    
    private readonly global::System.Collections.Generic.List<comrt.comnet.PRServer> _servers = new global::System.Collections.Generic.List<comrt.comnet.PRServer>();
    [global::ProtoBuf.ProtoMember(1, Name=@"servers", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<comrt.comnet.PRServer> servers
    {
      get { return _servers; }
    }
  
    private int _errorCode = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"errorCode", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int errorCode
    {
      get { return _errorCode; }
      set { _errorCode = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}