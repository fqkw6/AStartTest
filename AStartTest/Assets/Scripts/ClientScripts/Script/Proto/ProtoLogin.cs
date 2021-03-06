//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: ProtoLogin.proto
// Note: requires additional types generated from: ProtoBasis.proto
namespace comrt.comnet
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Account")]
  public partial class Account : global::ProtoBuf.IExtensible
  {
    public Account() {}
    
    private long _id = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long id
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
    private string _password = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"password", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string password
    {
      get { return _password; }
      set { _password = value; }
    }
    private string _email = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"email", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string email
    {
      get { return _email; }
      set { _email = value; }
    }
    private long _signup_time = default(long);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"signup_time", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long signup_time
    {
      get { return _signup_time; }
      set { _signup_time = value; }
    }
    private string _card_no = "";
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"card_no", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string card_no
    {
      get { return _card_no; }
      set { _card_no = value; }
    }
    private string _channel = "";
    [global::ProtoBuf.ProtoMember(12, IsRequired = false, Name=@"channel", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string channel
    {
      get { return _channel; }
      set { _channel = value; }
    }
    private string _phone_no = "";
    [global::ProtoBuf.ProtoMember(13, IsRequired = false, Name=@"phone_no", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string phone_no
    {
      get { return _phone_no; }
      set { _phone_no = value; }
    }
    private string _phone_model = "";
    [global::ProtoBuf.ProtoMember(14, IsRequired = false, Name=@"phone_model", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string phone_model
    {
      get { return _phone_model; }
      set { _phone_model = value; }
    }
    private string _phone_resolution = "";
    [global::ProtoBuf.ProtoMember(15, IsRequired = false, Name=@"phone_resolution", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string phone_resolution
    {
      get { return _phone_resolution; }
      set { _phone_resolution = value; }
    }
    private string _phone_os = "";
    [global::ProtoBuf.ProtoMember(16, IsRequired = false, Name=@"phone_os", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string phone_os
    {
      get { return _phone_os; }
      set { _phone_os = value; }
    }
    private string _phone_manufacturer = "";
    [global::ProtoBuf.ProtoMember(17, IsRequired = false, Name=@"phone_manufacturer", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string phone_manufacturer
    {
      get { return _phone_manufacturer; }
      set { _phone_manufacturer = value; }
    }
    private string _phone_imei = "";
    [global::ProtoBuf.ProtoMember(18, IsRequired = false, Name=@"phone_imei", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string phone_imei
    {
      get { return _phone_imei; }
      set { _phone_imei = value; }
    }
    private string _phone_mac = "";
    [global::ProtoBuf.ProtoMember(19, IsRequired = false, Name=@"phone_mac", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string phone_mac
    {
      get { return _phone_mac; }
      set { _phone_mac = value; }
    }
    private string _client_version = "";
    [global::ProtoBuf.ProtoMember(20, IsRequired = false, Name=@"client_version", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string client_version
    {
      get { return _client_version; }
      set { _client_version = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PModifyAccount")]
  public partial class PModifyAccount : global::ProtoBuf.IExtensible
  {
    public PModifyAccount() {}
    
    private long _userid = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userid", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userid
    {
      get { return _userid; }
      set { _userid = value; }
    }
    private string _oldName = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"oldName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string oldName
    {
      get { return _oldName; }
      set { _oldName = value; }
    }
    private string _oldPsw = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"oldPsw", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string oldPsw
    {
      get { return _oldPsw; }
      set { _oldPsw = value; }
    }
    private string _newName = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"newName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string newName
    {
      get { return _newName; }
      set { _newName = value; }
    }
    private string _newPsw = "";
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"newPsw", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string newPsw
    {
      get { return _newPsw; }
      set { _newPsw = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"AccountAnswer")]
  public partial class AccountAnswer : global::ProtoBuf.IExtensible
  {
    public AccountAnswer() {}
    
    private int _errorCode = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"errorCode", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int errorCode
    {
      get { return _errorCode; }
      set { _errorCode = value; }
    }
    private string _name = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private long _user_id = default(long);
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"user_id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long user_id
    {
      get { return _user_id; }
      set { _user_id = value; }
    }
    private string _session_key = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"session_key", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string session_key
    {
      get { return _session_key; }
      set { _session_key = value; }
    }
    private int _count = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"count", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int count
    {
      get { return _count; }
      set { _count = value; }
    }
    private readonly global::System.Collections.Generic.List<comrt.comnet.GameServer> _servers = new global::System.Collections.Generic.List<comrt.comnet.GameServer>();
    [global::ProtoBuf.ProtoMember(6, Name=@"servers", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<comrt.comnet.GameServer> servers
    {
      get { return _servers; }
    }
  
    private readonly global::System.Collections.Generic.List<int> _gmServerIds = new global::System.Collections.Generic.List<int>();
    [global::ProtoBuf.ProtoMember(7, Name=@"gmServerIds", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public global::System.Collections.Generic.List<int> gmServerIds
    {
      get { return _gmServerIds; }
    }
  
    private comrt.comnet.eAccountActivation _act = comrt.comnet.eAccountActivation.AACT_NONE;
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"act", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(comrt.comnet.eAccountActivation.AACT_NONE)]
    public comrt.comnet.eAccountActivation act
    {
      get { return _act; }
      set { _act = value; }
    }
    private bool _activation = default(bool);
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"activation", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(default(bool))]
    public bool activation
    {
      get { return _activation; }
      set { _activation = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"GameServer")]
  public partial class GameServer : global::ProtoBuf.IExtensible
  {
    public GameServer() {}
    
    private string _msgId = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"msgId", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string msgId
    {
      get { return _msgId; }
      set { _msgId = value; }
    }
    private int _id = default(int);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int id
    {
      get { return _id; }
      set { _id = value; }
    }
    private string _name = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string name
    {
      get { return _name; }
      set { _name = value; }
    }
    private string _ipv4 = "";
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"ipv4", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string ipv4
    {
      get { return _ipv4; }
      set { _ipv4 = value; }
    }
    private int _port = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"port", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int port
    {
      get { return _port; }
      set { _port = value; }
    }
    private comrt.comnet.eSrvBusyDegree _busy_degree = comrt.comnet.eSrvBusyDegree.UNKNOWN21;
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"busy_degree", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(comrt.comnet.eSrvBusyDegree.UNKNOWN21)]
    public comrt.comnet.eSrvBusyDegree busy_degree
    {
      get { return _busy_degree; }
      set { _busy_degree = value; }
    }
    private string _domain_name = "";
    [global::ProtoBuf.ProtoMember(8, IsRequired = false, Name=@"domain_name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string domain_name
    {
      get { return _domain_name; }
      set { _domain_name = value; }
    }
    private comrt.comnet.eServerType _serverType = comrt.comnet.eServerType.UNKNOWN20;
    [global::ProtoBuf.ProtoMember(9, IsRequired = false, Name=@"serverType", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(comrt.comnet.eServerType.UNKNOWN20)]
    public comrt.comnet.eServerType serverType
    {
      get { return _serverType; }
      set { _serverType = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PWhichGameSrv")]
  public partial class PWhichGameSrv : global::ProtoBuf.IExtensible
  {
    public PWhichGameSrv() {}
    
    private long _userId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userId
    {
      get { return _userId; }
      set { _userId = value; }
    }
    private comrt.comnet.GameServer _gameSrv = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"gameSrv", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public comrt.comnet.GameServer gameSrv
    {
      get { return _gameSrv; }
      set { _gameSrv = value; }
    }
    private string _userName = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"userName", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string userName
    {
      get { return _userName; }
      set { _userName = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"Authentication")]
  public partial class Authentication : global::ProtoBuf.IExtensible
  {
    public Authentication() {}
    
    private int _errorCode = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"errorCode", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int errorCode
    {
      get { return _errorCode; }
      set { _errorCode = value; }
    }
    private string _session_id = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"session_id", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string session_id
    {
      get { return _session_id; }
      set { _session_id = value; }
    }
    private string _uid = "";
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"uid", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string uid
    {
      get { return _uid; }
      set { _uid = value; }
    }
    private comrt.comnet.Account _account = null;
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"account", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public comrt.comnet.Account account
    {
      get { return _account; }
      set { _account = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PCheckSession")]
  public partial class PCheckSession : global::ProtoBuf.IExtensible
  {
    public PCheckSession() {}
    
    private long _userId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userId
    {
      get { return _userId; }
      set { _userId = value; }
    }
    private string _session_key = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"session_key", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string session_key
    {
      get { return _session_key; }
      set { _session_key = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"PJoinGame")]
  public partial class PJoinGame : global::ProtoBuf.IExtensible
  {
    public PJoinGame() {}
    
    private long _roleId = default(long);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"roleId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long roleId
    {
      get { return _roleId; }
      set { _roleId = value; }
    }
    private long _userId = default(long);
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"userId", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(long))]
    public long userId
    {
      get { return _userId; }
      set { _userId = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}