using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAI : MonoBehaviour
{
  private int MAX_DEPTH = 5;

  void putMap(int[,] map,int[] node,int turn)
  {
    if (map[node[0], 2] != 0) {
      map[node[0],2] = 0;
    }else if (map[node[0], 1] != 0){
      map[node[0],1] = 0;
    } else if(map[node[0],0] != 0){
      map[node[0],0] = 0;
    }
    if(map[node[1],1]!=0){
      map[node[1],2] = turn;
    }else if(map[node[1],0] != 0){
      map[node[1],1] = turn;
    }else{
      map[node[1],0] = turn;
    }
  }

  int[,] getNodeList_advanced(int[,] map,int turn)
  {
      List<int> nd1list = new List<int>();
      List<int> nd2list = new List<int>();
      int maxValue = 0;
      for(int h = 0; h < 40; h++)
      {
        // 自分のゴールをスキップ
        if (h < 5) continue;
        if(map[h,2] == turn || (map[h,2] == 0 && map[h,1] == turn) || (map[h,1] == 0 && map[h,0] == turn)){
          for (int i = -1; i <= 1; i++)
          {
            for (int j = -1; j <= 1; j++)
            {
              // 枠外と現在のマスをスキップ
              if ((h < 5 && i == -1) || (24 < h && i == 1)) continue;
              if ((h % 5 == 0 && j == -1) || (h % 5 == 4 && j == 1)) continue;
              if (i == 0 && j == 0) continue;

              int p = h + 5 * i + j;
              if (map[p, 2] == 0)
              {
                  nd1list.Add(h);
                  nd2list.Add(p);
              }
            }
          }
        }
      }
      int numberOfNodes = nd1list.Count;
      int[,] nodeList = new int[numberOfNodes,2];

      int[] valueList_sort = new int[numberOfNodes];
      int[] valueList_org = new int[numberOfNodes];

      for(int i = 0;i<numberOfNodes;i++){
        nodeList[i,0] = nd1list[i];
        nodeList[i,1] = nd2list[i];
        int[] nd = {nd1list[i],nd2list[i]};

        putMap(map,nd,turn);

        int value = evalMap(map);
        valueList_org[i] = value;
        valueList_sort[i] = value;
        Array.Reverse(nd);

        putMap(map,nd,turn);
      }

      Array.Sort(valueList_sort);
      if(turn == 1){
        Array.Reverse(valueList_sort);
      }
      int[,] nodeList_sorted = new int[numberOfNodes,2];
      for(int i = 0;i<numberOfNodes;i++){
        int index = Array.IndexOf(valueList_org ,valueList_sort[i]);
        nodeList_sorted[i,0] = nodeList[index,0];
        nodeList_sorted[i,1] = nodeList[index,1];

        // 同じ評価値が続いたときにハンドが重複しないようにするため
        // 例えばvalueList_sort = [6,6,5,3,2,1]
        // valueLis_org = [3,2,5,6,1,6] ここで評価値６のハンドを代入した後
        // -> valueList_org = [3,2,5,7,1,6] という風に値を変えると同じハンドを代入してしまうことがなくなる。
        if(turn == 1){
          valueList_org[index] += 1;
        }else{
          valueList_org[index] -= 1;
        }
      }

      return nodeList_sorted;
  }

  int[,] getNodeList(int[,] map,int turn)
  {
      List<int> nd1list = new List<int>();
      List<int> nd2list = new List<int>();
      int maxValue = 0;
      for(int h = 0; h < 30; h++)
      {
        // 自分のゴールをスキップ
        if (h < 5) continue;
        if(map[h,2] == turn || (map[h,2] == 0 && map[h,1] == turn) || (map[h,1] == 0 && map[h,0] == turn)){
          for (int i = -1; i <= 1; i++)
          {
            for (int j = -1; j <= 1; j++)
            {
              // 枠外と現在のマスをスキップ
              if ((h < 5 && i == -1) || (24 < h && i == 1)) continue;
              if ((h % 5 == 0 && j == -1) || (h % 5 == 4 && j == 1)) continue;
              if (i == 0 && j == 0) continue;

              int p = h + 5 * i + j;
              if (map[p, 2] == 0)
              {
                  nd1list.Add(h);
                  nd2list.Add(p);
              }
            }
          }
        }
      }
      int numberOfNodes = nd1list.Count;
      int[,] nodeList = new int[numberOfNodes,2];
      for(int i = 0;i<numberOfNodes;i++){
        nodeList[i,0] = nd1list[i];
        nodeList[i,1] = nd2list[i];
      }

      return nodeList;
  }

  int[,] getNodeList_forward(int[,] map,int turn)
  {
    List<int> nd1list = new List<int>();
    List<int> nd2list = new List<int>();
    int maxValue = 0;
    for(int h = 0; h < 30; h++)
    {
      if(map[h,2] == turn || (map[h,2] == 0 && map[h,1] == turn) || (map[h,1] == 0 && map[h,0] == turn)){
        for (int i = -1; i <= 1; i++)
        {
          for (int j = -1; j <= 1; j++)
          {
            // 枠外と現在のマスをスキップ
            if ((h < 5 && i == -1) || (24 < h && i == 1)) continue;
            if ((h % 5 == 0 && j == -1) || (h % 5 == 4 && j == 1)) continue;
            if (i == 0 && j == 0) continue;

            int p = h + 5 * i + j;
            if (map[p, 2] == 0)
            {
                nd1list.Add(h);
                nd2list.Add(p);
            }
          }
        }
      }
    }
    int numberOfNodes = nd1list.Count;
    int[,] nodeList = new int[numberOfNodes,2];

    int[] valueList_sort = new int[numberOfNodes];
    int[] valueList_org = new int[numberOfNodes];

    for(int i = 0;i<numberOfNodes;i++){
      nodeList[i,0] = nd1list[i];
      nodeList[i,1] = nd2list[i];

      valueList_org[i] = nd2list[i];
      valueList_sort[i] = nd2list[i];
    }

    Array.Sort(valueList_sort);
    if(turn == -1){
      Array.Reverse(valueList_sort);
    }
    int[,] nodeList_sorted = new int[numberOfNodes,2];
    for(int i = 0;i<numberOfNodes;i++){
      int index = Array.IndexOf(valueList_org ,valueList_sort[i]);
      nodeList_sorted[i,0] = nodeList[index,0];
      nodeList_sorted[i,1] = nodeList[index,1];

      // 同じ評価値が続いたときにハンドが重複しないようにするため
      // 例えばvalueList_sort = [6,6,5,3,2,1]
      // valueLis_org = [3,2,5,6,1,6] ここで評価値６のハンドを代入した後
      // -> valueList_org = [3,2,5,7,1,6] という風に値を変えると同じハンドを代入してしまうことがなくなる。
      if(turn == 1){
        valueList_org[index] -= 1;
      }else{
        valueList_org[index] += 1;
      }
    }

    return nodeList_sorted;
  }

  int evalMap(int[,] map)
  {
    int ev = 0;

    for(int i = 0; i < 30; i++)
    {

        if(map[i,0] == 0)continue;

        // 相手の駒二つの上に乗るとかなり有利
        if(map[i,2] == 1 && map[i,1] == -1 && map[i,0] == -1)
        {
            ev += 5;
        }
        else if(map[i,2] == -1 && map[i,1] == 1 && map[i,0] == 1)
        {
            ev -= 5;
        }

        // 相手の上に乗っているとそこそこ有利
        if(map[i,0] == 1 && map[i,1] == -1){
          ev -= 3;
        }
        else if(map[i,0] == -1 && map[i,1] == 1){
          ev += 3;
        }

        // 駒が前にあるほど有利
        for(int f = 0;f < 3;f++){
          if(map[i,f] == 1){
            if(i/5 == 1)ev += 2;
            if(i/5 == 2)ev += 4;
            ev += 5 - i/5;
          }else if(map[i,f] == -1){
            if(i/5 == 4)ev -= 2;
            if(i/5 == 5)ev -= 4;
            ev -= i/5;
          }
        }


    }
    return ev;

  }

  public (int[],int) thinkHand(int[,] map,int moveNumber)
  {
    int depth = 1;
    if(moveNumber == 0){
      depth = 1;
    }else if(moveNumber < MAX_DEPTH){
      depth = moveNumber;
    }else{
      depth = MAX_DEPTH;
    }
    var sw = new System.Diagnostics.Stopwatch();
    sw.Start();

    var(hand, sc) = deepThink_ab(map, -1, depth, 9999 * -1, 9999);
    sw.Stop();
    TimeSpan ts = sw.Elapsed;
    Debug.Log("思考時間: "+ts.Minutes+"分" + ts.Seconds + "秒");
    int ev = evalMap(map);
    Debug.Log("ev: " + ev);
    return (hand,sc);
  }

  (int[],int)deepThink_ab(int[,] map,int turn,int depth,int a,int b)
  {
    int[] besthand = null;

    for (int i = 0;i<40;i++){
      if (i < 6 && turn == 1)
      {
          if (map[i, 2] == 1 || (map[i, 2] == 0 && map[i, 1] == 1) || (map[i, 2] == 0 && map[i, 1] == 0 && map[i, 0] == 1))
          {
              return(null,999);
          }
      }
      else if (29 < i && turn == -1)
      {
          if (map[i, 2] == -1 || (map[i, 2] == 0 && map[i, 1] == -1) || (map[i, 2] == 0 && map[i, 1] == 0 && map[i, 0] == -1))
          {
            return(null,-999);
          }
      }
    }

    if(depth == 0)
    {
        return (null, evalMap(map));
    }

    int[,] nodeList = getNodeList(map, turn);
    if(depth > 3 && depth <9){
      nodeList = getNodeList_advanced(map,turn);
    }

    for(int i = 0;(a < b) && (i < nodeList.GetLength(0)); i++)
    {
        int[] hand = {nodeList[i,0],nodeList[i,1]};
        int[] hand_reverse = {nodeList[i,1],nodeList[i,0]};

        putMap(map, hand,turn);


        var (bHand,sc) = deepThink_ab(map, turn * -1, depth - 1, a, b);

        // ここでマップを戻す
        putMap(map,hand_reverse,turn);

        // if(besthand == null)
        // {
        //     besthand = hand;
        //     if(turn == 1)a = sc;
        //     else b = sc;
        // }

        if (turn == 1 && sc > a)
        {
            a = sc;
            besthand = hand;
        }
        else if (turn == -1 && sc < b)
        {
            b = sc;
            besthand = hand;
        }
    }
    int bestscore = 0;
    if(turn == 1)bestscore = a;
    if(turn == -1)bestscore = b;
    return (besthand, bestscore);
  }
}
