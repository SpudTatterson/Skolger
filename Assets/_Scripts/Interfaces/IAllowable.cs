using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAllowable 
{
   void OnAllow();
   void OnDisallow();

   bool IsAllowed();
}
