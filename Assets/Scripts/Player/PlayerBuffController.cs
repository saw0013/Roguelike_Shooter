using System;
using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using UnityEngine;

public class PlayerBuffController : MonoBehaviour
{

    private float _delayTimeNewBuff = 10; //Время задержки перед взятием нового бафа, чтобы за 10 секунд не бралось более 1 бафа

    private bool _giveBuffNow = true; //Проверим можно ли подобрать баф

    private List<string> _buffs = new List<string>();


    /// <summary>
    ///  Проверяет есть ли на игроке баф
    /// <para>Если есть то не будем дублировать UI а просто обновим время</para>
    /// </summary>
    /// <param name="NameBuff">Указать через nameof(Class)</param>
    /// <returns>TRUE если баф есть</returns>
    public bool BuffIsExist(string NameBuff)
    {
        if (_buffs.Contains(NameBuff)) //Проверим есть\был ли баф на игроке
            return true;
        else
        {
            _buffs.Add(NameBuff);
            return false;
        }
    }

    /// <summary>
    /// Возвращает TRUE если можно подобрать
    /// <para>FALSE если просто уничтожаем</para>
    /// </summary>
    /// <returns></returns>
    public bool PickOrDestroy()
    {
        if (_giveBuffNow) return true;
        else return false;
    }


    private void Update()
    {
        if (!_giveBuffNow)
        {
            if (_delayTimeNewBuff > 0)
                _delayTimeNewBuff -= Time.deltaTime;
            else
            {
                _giveBuffNow = true;
                _delayTimeNewBuff = 10;
            }
        }
    }
}
