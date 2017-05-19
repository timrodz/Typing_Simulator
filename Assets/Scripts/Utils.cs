using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Utils {

    public static string ColorToHex (Color color) {

		return ColorUtility.ToHtmlStringRGB(color);

    }

}