using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData //플레이어 데이터 클래스
{
    public int money; //가진 돈 변수
    public List<int> skinList; //가진 스킨 리스트 변수
    public int currentSkinNum; //현재 스킨 번호 변수
    public int unlockedStageCount; //해금한 스테이지 개수 변수
    public List<int> stageScoreList; //스테이지 점수 리스트 변수

	public PlayerData() //플레이어 데이터 생성자 : 플레이어가 처음 플레이하는 경우 데이터가 없기 때문
	{
        money = 0; //가진 돈 저장
		skinList = new List<int>(); //스킨 리스트 저장
        skinList.Add(0); //0번 스킨은 기본 해금
        currentSkinNum = 0; //현재 스킨 저장
        unlockedStageCount = 0; //스테이지 1은 기본 해금
		stageScoreList = new List<int>(); //스테이지 점수 리스트 저장
        stageScoreList.Add(0); //처음 플레이하면 0점
	}

	#region Getter/Setter
	public void SetMoney(int money) //가진 돈 저장 메소드
    {
        this.money = money;
    }
    public int GetMoney() //가진 돈 반환 메소드
    {
        return money;
    }

    public void SetSkinList(List<int> skinList) //스킨 리스트 저장 메소드
    {
        this.skinList = skinList;
    }
    public List<int> GetSkinList() //스킨 리스트 반환 메소드
    {
       return skinList;
    }

    public void SetUnlockedStageCount(int unlockedStageCount) //해금한 스테이지 개수 저장 메소드
    {
        this.unlockedStageCount = unlockedStageCount;
    }
    public int GetUnlockedStageCount() //해금한 스테이지 개수 반환 메소드
    {
         return unlockedStageCount;
    }

    public void SetStageScoreList(List<int> stageScoreList) //스테이지 점수 리스트 저장 메소드
    {  
        this.stageScoreList = stageScoreList;
    }
    public List<int> GetStageScoreList() //스테이지 점수 리스트 반환 메소드
    {
        return stageScoreList;
    }

	#endregion

    public void AddStageScoreList() //스테이지 점수 리스트 추가 메소드 : 업데이트로 스테이지가 추가되어도 플레이어 데이터에는 영향이 없기 때문에
                                    //점수 리스트에 0점을 추가하는 용도
    {
        stageScoreList.Add(0); //스테이지 점수 리스트 추가
    }

	public void AddSkinNum(int skinNum) //가진 스킨 추가 메소드
    {
        skinList.Add(skinNum); //가진 스킨 번호 추가
        skinList.Sort(); //스킨은 무작위적으로 구매할 수 있기 때문에 리스트 정리
    }

    public void AddUnlockedStageCount() //해금한 스테이지 추가 메소드
    {
        unlockedStageCount++; //해금한 스테이지 개수 추가
    }

    public void AddMoney(int money)
    {
        this.money += money;
    }

	public void SetStageScore(int stageNum, int stageScore) //스테이지 클리어 후 점수 저장 메소드
	{
		stageScoreList[stageNum] = stageScore; //스테이지 점수 저장
	}

}
