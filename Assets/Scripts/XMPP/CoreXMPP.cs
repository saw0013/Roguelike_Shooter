using agsXMPP.protocol.client;
using agsXMPP.protocol.iq.register;
using agsXMPP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderData;
using agsXMPP.protocol.iq.roster;
using System;
using agsXMPP.protocol.x.muc;
using agsXMPP.Xml.Dom;
using UnityEditor.XR;
using System.Net.Mail;
using agsXMPP.protocol.x.muc.iq.admin;

public class CoreXMPP
{
    private XmppClientConnection xmppCon;

    /// <summary>
    /// Создаёт подключение с выбранным логином и паролем при инициализации экземпляра класса
    /// </summary>
    /// <param name="username"></param>
    /// <param name="pass"></param>
    public CoreXMPP(string username, string pass)
    {
        xmppCon = new XmppClientConnection("develop.blueboxproduction.ru");
        xmppCon.Open(username, pass);
        //xmppCon.OnLogin += new ObjectHandler(xmppCon_OnLogin);
        //xmppCon.OnAuthError += new XmppElementHandler(xmppCon_OnAuthError);
        //xmppCon.OnError += new ErrorHandler(xmppCon_OnError);
        //xmppCon.OnClose += new ObjectHandler(xmppCon_OnClose);
        //xmppCon.OnMessage += new agsXMPP.protocol.client.MessageHandler(xmppCon_OnMessage);

    }
    /// <summary>
    /// <para>Регистрируем нового пользователя.</para>
    /// <para>Статический класс чтобы можно было объявить без начала игры</para>
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="email">Не обязательный параметр</param>
    public static void CreateAccount(string username, string password, string email = "user@blueboxproduction.ru")
    {
        Jid jid = new Jid("admin@develop.blueboxproduction.ru");
        XmppClientConnection client = new XmppClientConnection(jid.Server);
        client.Username = jid.User;
        client.Password = "Nadeev!2116";
        client.Open();

        // Создаем новую комнату
        MucManager mucManager = new MucManager(client);
        Jid roomJid = new Jid("myroom@share.develop.blueboxproduction.ru");
        mucManager.JoinRoom(roomJid, "BUAFA");
        mucManager.CreateReservedRoom(roomJid);

     
        // Отключаемся от сервера XMPP
        Debug.LogWarning("КОМНАТУШКА");
        //client.Close();

        //Jid jid = new Jid("test01@develop.blueboxproduction.ru");
        //XmppClientConnection xmpp = new XmppClientConnection(jid.Server);
        //xmpp.Username = jid.User;
        //xmpp.Password = "Pa$$w0rD";
        //xmpp.Open();
        //
        //xmpp.OnRosterStart += new ObjectHandler(xmppCon_OnRosterStart);
        //xmpp.OnRosterItem += new XmppClientConnection.RosterHandler(xmppCon_OnRosterItem);
        //xmpp.OnRosterEnd += new ObjectHandler(xmppCon_OnRosterEnd);
        // создание запроса на регистрацию
        //RegisterIq reg = new RegisterIq(IqType.set);
        //reg.Query.Username = username;
        //reg.Query.Password = password;
        //reg.Query.Email = email;

        //// отправка запроса на сервер
        //xmpp.Send(reg);

    }


    private static void xmppCon_OnRosterEnd(object sender)
    {
        Debug.LogWarning("END");
    }

    private static void xmppCon_OnRosterStart(object sender)
    {
        Debug.LogWarning("START");
    }

    private static void xmppCon_OnRosterItem(object sender, RosterItem item)
    {
        Debug.LogWarning(String.Format("Got contact: {0}", item.Name));
    }
}
