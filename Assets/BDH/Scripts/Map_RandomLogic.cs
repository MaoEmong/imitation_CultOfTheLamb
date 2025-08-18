using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map_RandomLogic : MonoBehaviour
{
    public struct mapData
    {
		public mapData(int num, int up, int left, int right, int down)
        {
            roomNum = num;

            isActive = false;
			isClear = false;

			upDoor = up;
			leftDoor = left;
			rightDoor = right;
			downDoor = down;
		}

		public int roomNum;

        public bool isActive;
        public bool isClear;

        public int upDoor;
        public int leftDoor;
        public int rightDoor;
        public int downDoor;
    }

	mapData roomStart = new mapData(1, 0, 0, 0, 0);
	mapData roomTwo = new mapData(2, 0, 0, 0, 0);
	mapData roomThree = new mapData(3, 0, 0, 0, 0);
	mapData roomFour = new mapData(4, 0, 0, 0, 0);
	mapData roomFive = new mapData(5, 0, 0, 0, 0);
	mapData roomSix = new mapData(6, 0, 0, 0, 0);
	mapData roomSeven = new mapData(7, 0, 0, 0, 0);
	mapData roomEight = new mapData(8, 0, 0, 0, 0);
	mapData roomNine = new mapData(9, 0, 0, 0, 0);
	mapData roomTen = new mapData(10, 0, 0, 0, 0);
	mapData roomEleven = new mapData(11, 0, 0, 0, 0);
	mapData roomEnd = new mapData(12, 0, 0, 0, 0);

	public mapData[] nowMap = new mapData[12];

	public void randomMapMake()
    {
        nowMap[0] = roomStart;

        for (int i = 1; i < 12; i++)
        {

            
        }
	}

    void roomConnectNum()
    {
		int roomNum = 0;

		
    }
}
