﻿/*****************************************
 * This file is part of Impulse Framework.

    Impulse Framework is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Impulse Framework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with Impulse Framework.  If not, see <http://www.gnu.org/licenses/>.
*****************************************/

using UnityEngine;
using System.Collections;

/// <summary>
/// Attach to anything that needs to have a faction specified.
/// </summary>
[System.Serializable]
public class Faction : MonoBehaviour
{
    // Once every faction is defined this should be replaced with an enum (much more efficient)
    [SerializeField]
    Factions factionName;
    public Factions FactionName
    {
        get { return factionName; }
        private set { factionName = value; }
    }


    // Modify this to add more factions
    public enum Factions
    {
        Players,
        Enemies
    }
}
