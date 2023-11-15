using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CustomEditor
{
    public class TestComponent : MonoBehaviour
    {

        /* public float a = 1;
        [SerializeField]
        private int b = 2;
        private float c = 3;
        public bool d = true;
        [SerializeField]
        protected long l = 20;
        public TestEnum e = TestEnum.second; */
        public GameObject go;
    }
    public enum TestEnum
    {
        first = 0,
        second = 1,
        third = 2
    }
}