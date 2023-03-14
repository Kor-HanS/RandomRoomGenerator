using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GenerateRandomRoom : MonoBehaviour
{
    // 방 오브젝트 리스트 
    [SerializeField]
    private GameObject[] roomObjects;
    int[] direction = {-1,1,-10,10};
    List<int> pickedRoomResult = new List<int>();
    bool isRandomRoomGenerated = false;
    List<int> pickedEndRoom = new List<int>();
    int maxDiff = 0;
    int bossRoomNum = 0;

    // 아이작 방 생성 알고리즘 설명
    // 현재셀에서 +10,-10,+1,-1 로 이동하며 방 넓히기.
    // 이동한 방이
    // 1. 이미 점유되있음 포기.
    // 2, 이동한 방의 점유된 이웃이 1개 보다 많다 포기.
    // 3. 이미 정해진 수 만큼의 방 생성 되있다면 포기.
    // 4. 50% 확률로 포기.
    // 5. 위 조건에서 걸러지지 않았다면 방 넓힌후, 큐에 넣기.

    // 추가적으로, 끝방 중에서 시작점과 가장 먼 방을 보스방으로 설정한다.
    // 1. 보스 방 선정.
    // 2. 끝방 중 보스방이 아닌 방 골드 방과 상점방으로 선정.

    private void Start() {

        do{
            // 55번방을 시작으로 25개의 방을 선택. 실패할경우 다시 생성 시도.
            isRandomRoomGenerated = PickRandomRoom(25,55);
        }while(!isRandomRoomGenerated);

        // 시작 방 색깔 gray
        roomObjects[55].GetComponent<RawImage>().color = Color.green;

        for(int i = 0; i < pickedRoomResult.Count; i++){
            roomObjects[pickedRoomResult[i]].SetActive(true);
            // 끝방 처리.
            if(neighborRoomOccupied(pickedRoomResult[i], pickedRoomResult) == 1){
                pickedEndRoom.Add(pickedRoomResult[i]);
                int t_dist = distance(pickedRoomResult[i], 55);
                if(maxDiff < t_dist){
                    maxDiff = t_dist;
                    bossRoomNum = pickedRoomResult[i]; // 55번 시작 방과 가장 먼 방 보스방 선정.
                }
            }
        }
        roomObjects[bossRoomNum].GetComponent<RawImage>().color = Color.red;
        
        int t_roomNum1,t_roomNum2;
        do{
            t_roomNum1 = pickedEndRoom[Random.Range(0,pickedEndRoom.Count)];
            t_roomNum2 = pickedEndRoom[Random.Range(0,pickedEndRoom.Count)];

            if(t_roomNum1 == bossRoomNum || t_roomNum2 == bossRoomNum){continue;}
            else if(t_roomNum1 == 55 || t_roomNum2 == 55){continue;}
            else if(t_roomNum1 != t_roomNum2){
                // 골드방 설정.
                roomObjects[t_roomNum1].GetComponent<RawImage>().color = Color.yellow;
                // 상점방 설정.
                roomObjects[t_roomNum2].GetComponent<RawImage>().color = Color.blue;
                break;
            }
        }while(true);


    }

    bool PickRandomRoom(int size, int startRoomNum){
        
        List<int> pickedRooms = new List<int>(); // 현재 뽑힌 방의 번호들.
        Queue<int> roomQueue = new Queue<int>();
        
        int[] direction = {-1,1,-10,10};
        
        roomQueue.Enqueue(startRoomNum);
        pickedRooms.Add(startRoomNum);
        
        while(roomQueue.Count != 0){
            int now = roomQueue.Dequeue();

            for(int i = 0 ; i < 4; i++){
                int next = now + direction[i];
                
                if(!inside(next)){continue;} // 0 ~ 99 번 방만 탐색.
                else if((now/10) != (next/10) && (i == 0 || i == 1)){continue;} // 일차원 배열이라서 다른 -1이나 +1로 다른 row 가면 그림 안이쁘게 나옴. 
                else if(pickedRooms.Contains(next)){continue;} // 이미 점유 된 방 포기.
                else if(neighborRoomOccupied(next,pickedRooms) > 1){continue;} // 탐색한 방 옆에 점유된 방이 1개 보다 많으면 포기.
                else if(Random.Range(0,2) == 1){continue;} // 0과 1 중에 1이 나오면 포기. 50% 확률.
                else{
                    pickedRooms.Add(next);
                    roomQueue.Enqueue(next);
                    if(pickedRooms.Count >= size){pickedRoomResult = pickedRooms; return true;} // 이미 뽑은 방이 충분하면 끝.
                }
            }
        }
        // 방생성 실패.
        return false;
    }

    int neighborRoomOccupied(int now, List<int> pickedRooms){
        int num = 0;
        for(int i = 0; i < 4; i++){
            int next = now + direction[i];
            if(inside(now) && pickedRooms.Contains(next)){num++;}
        }
        return num;
    }

    bool inside(int now){
        return(now < 100 && now >= 0);
    }
 
    int distance(int num1, int num2){
        int diff_10 = Mathf.Abs((num1 / 10) - (num2 / 10));
        int diff_1 = Mathf.Abs((num1 % 10) - (num2 % 10));
        return (diff_10 + diff_1);
    }

}
