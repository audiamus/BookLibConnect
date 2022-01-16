namespace core.audiamus.adb.json {
  public class RegistrationResponse : Serialization<RegistrationResponse> {
    public Response response { get; set; }
    public string request_id { get; set; }
  }

  public class Response {
    public Success success { get; set; }
  }

  public class Success {
    public Extensions extensions { get; set; }
    public Tokens tokens { get; set; }
    public string customer_id { get; set; }
  }

  public class Extensions {
    public Device_Info device_info { get; set; }
    public Customer_Info customer_info { get; set; }
  }

  public class Device_Info {
    public string device_name { get; set; }
    public string device_serial_number { get; set; }
    public string device_type { get; set; }
  }

  public class Customer_Info {
    public string account_pool { get; set; }
    public string user_id { get; set; }
    public string home_region { get; set; }
    public string name { get; set; }
    public string given_name { get; set; }
  }

  public class Tokens {
    public Website_Cookies[] website_cookies { get; set; }
    public Store_Authentication_Cookie store_authentication_cookie { get; set; }
    public Mac_Dms mac_dms { get; set; }
    public Bearer bearer { get; set; }
  }

  public class Store_Authentication_Cookie {
    public string cookie { get; set; }
  }

  public class Mac_Dms {
    public string device_private_key { get; set; }
    public string adp_token { get; set; }
  }

  public class Bearer {
    public string access_token { get; set; }
    public string refresh_token { get; set; }
    public string expires_in { get; set; }
  }

  public class Website_Cookies {
    public string Path { get; set; }
    public string Secure { get; set; }
    public string Value { get; set; }
    public string Expires { get; set; }
    public string Domain { get; set; }
    public string HttpOnly { get; set; }
    public string Name { get; set; }
  }

}
