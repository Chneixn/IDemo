using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityUtils
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 遍历子物体，返回仅含子物体的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <returns>仅含子物体的数组</returns>
        public static T[] GetComponentsInRealChildren<T>(this Transform go) where T : Object
        {
            //创建一个新列表，去掉第一个就是去掉自身
            List<T> TList = new List<T>();
            TList.AddRange(go.GetComponentsInChildren<T>());    //AddRange 将数组填充至列表
            TList.RemoveAt(0);
            return TList.ToArray();
        }

        /// <summary>
        /// 遍历子物体，返回仅含子物体的数组(包括未激活的子物体)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="go"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static T[] GetComponentsInRealChildren<T>(this Transform go, bool includeInactive = false) where T : Object
        {

            List<T> TList = new List<T>();
            TList.AddRange(go.GetComponentsInChildren<T>(includeInactive));
            TList.RemoveAt(0);
            return TList.ToArray();
        }

        /// <summary>
        /// 在SetActive前检测Active状态，避免重复调用OnEnable或OnDisable
        /// </summary>
        /// <param name="go"></param>
        /// <param name="state"></param>
        public static void SetActiveSafe(this GameObject go, bool state = true)
        {
            if (go == null)
            {
                return;
            }

            if (go.activeSelf != state)
            {
                go.SetActive(state);
            }
        }
    }
}


