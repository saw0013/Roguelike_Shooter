using System.Collections.Generic;

public class PubAccount
{
    public string id { get; set; }
    public string login { get; set; }
    public string passwd { get; set; }
    public string email { get; set; }
    public string mobile_number { get; set; }
    public string rank { get; set; }
    public string ipaddr { get; set; }
    public string last_login { get; set; }
}



public class MyGame
{
    public string id { get; set; }
    public string id_purchase { get; set; }
    public string coins { get; set; }
}

public class MyPurchase
{
    public string id { get; set; }
    public string id_user { get; set; }
    public string id_game { get; set; }
    public string date_purchase { get; set; }
    public string price { get; set; }
}

public class RootData
{

    public PubAccount pubAccount { get; set; }
    public List<MyGame> myGame { get; set; }
    public List<MyPurchase> myPurchase { get; set; }
}
