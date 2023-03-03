using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.register;
using agsXMPP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreXMPP
{
    public static void CreateAccount(string username, string password, string email = "user@blueboxproduction.ru")
    {
        Jid jid = new Jid("admin@develop.blueboxproduction.ru");
        XmppClientConnection xmpp = new XmppClientConnection(jid.Server);
        xmpp.Username = jid.User;
        xmpp.Password = "password";
        xmpp.Open();

        // создание запроса на регистрацию
        RegisterIq reg = new RegisterIq(IqType.set);
        reg.Query.Username = username;
        reg.Query.Password = password;
        reg.Query.Email = email;

        // отправка запроса на сервер
        xmpp.Send(reg);
    }
}
