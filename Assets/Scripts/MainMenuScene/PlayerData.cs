using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData //�÷��̾� ������ Ŭ����
{
    public int money; //���� �� ����
    public List<int> skinList; //���� ��Ų ����Ʈ ����
    public int currentSkinNum; //���� ��Ų ��ȣ ����
    public int unlockedStageCount; //�ر��� �������� ���� ����
    public List<int> stageScoreList; //�������� ���� ����Ʈ ����

	public PlayerData() //�÷��̾� ������ ������ : �÷��̾ ó�� �÷����ϴ� ��� �����Ͱ� ���� ����
	{
        money = 0; //���� �� ����
		skinList = new List<int>(); //��Ų ����Ʈ ����
        skinList.Add(0); //0�� ��Ų�� �⺻ �ر�
        currentSkinNum = 0; //���� ��Ų ����
        unlockedStageCount = 0; //�������� 1�� �⺻ �ر�
		stageScoreList = new List<int>(); //�������� ���� ����Ʈ ����
        stageScoreList.Add(0); //ó�� �÷����ϸ� 0��
	}

	#region Getter/Setter
	public void SetMoney(int money) //���� �� ���� �޼ҵ�
    {
        this.money = money;
    }
    public int GetMoney() //���� �� ��ȯ �޼ҵ�
    {
        return money;
    }

    public void SetSkinList(List<int> skinList) //��Ų ����Ʈ ���� �޼ҵ�
    {
        this.skinList = skinList;
    }
    public List<int> GetSkinList() //��Ų ����Ʈ ��ȯ �޼ҵ�
    {
       return skinList;
    }

    public void SetUnlockedStageCount(int unlockedStageCount) //�ر��� �������� ���� ���� �޼ҵ�
    {
        this.unlockedStageCount = unlockedStageCount;
    }
    public int GetUnlockedStageCount() //�ر��� �������� ���� ��ȯ �޼ҵ�
    {
         return unlockedStageCount;
    }

    public void SetStageScoreList(List<int> stageScoreList) //�������� ���� ����Ʈ ���� �޼ҵ�
    {  
        this.stageScoreList = stageScoreList;
    }
    public List<int> GetStageScoreList() //�������� ���� ����Ʈ ��ȯ �޼ҵ�
    {
        return stageScoreList;
    }

	#endregion

    public void AddStageScoreList() //�������� ���� ����Ʈ �߰� �޼ҵ� : ������Ʈ�� ���������� �߰��Ǿ �÷��̾� �����Ϳ��� ������ ���� ������
                                    //���� ����Ʈ�� 0���� �߰��ϴ� �뵵
    {
        stageScoreList.Add(0); //�������� ���� ����Ʈ �߰�
    }

	public void AddSkinNum(int skinNum) //���� ��Ų �߰� �޼ҵ�
    {
        skinList.Add(skinNum); //���� ��Ų ��ȣ �߰�
        skinList.Sort(); //��Ų�� ������������ ������ �� �ֱ� ������ ����Ʈ ����
    }

    public void AddUnlockedStageCount() //�ر��� �������� �߰� �޼ҵ�
    {
        unlockedStageCount++; //�ر��� �������� ���� �߰�
    }

    public void AddMoney(int money)
    {
        this.money += money;
    }

	public void SetStageScore(int stageNum, int stageScore) //�������� Ŭ���� �� ���� ���� �޼ҵ�
	{
		stageScoreList[stageNum] = stageScore; //�������� ���� ����
	}

}
