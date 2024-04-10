using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO.Pipes;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
  public Material selectingMat;
  public Material whitemat;
  public Material pinkmat;
  public GameObject enemy;
  public GameObject noccaPrefab;
  public GameObject enemyNoccaPrefab;
  public GameObject messageText;
  private GameObject selecting = null;
  private int moved = 0;
  private Boolean isEnemyThinking = false;
  private int winner = 0;

  int[,] _map = {
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {-1,0,0},{-1,0,0},{-1,0,0},{-1,0,0},{-1,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {1,0,0},{1,0,0},{1,0,0},{1,0,0},{1,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
    };

  // Start is called before the first frame update
  void Update()
  {
    if (winner != 0){
      Text text = messageText.GetComponent<Text>();
      text.text = (winner > 0 ? "You " : "Enemy ") + "Win!";
    }
    else if (isEnemyThinking)
    {
      Text text = messageText.GetComponent<Text>();
      text.text = "Thinking...";
      EnemyAction();
      text.text = "";
      isEnemyThinking = false;
    }
    else if (Input.GetMouseButtonDown(0))
    {
      HandleMouseClick();
    }
  }

  private void HandleMouseClick()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit, Mathf.Infinity))
    {
      if (hit.collider.gameObject.CompareTag("Squares"))
      {
        if (selecting == null)
        {
          selecting = hit.collider.gameObject;
          selecting.GetComponent<Renderer>().material = selectingMat;
        }
        else
        {
          int[] nd = { int.Parse(selecting.name), int.Parse(hit.collider.gameObject.name) };
          if (int.Parse(selecting.name) % 2 == 0) selecting.GetComponent<Renderer>().material = whitemat;
          else selecting.GetComponent<Renderer>().material = pinkmat;
          selecting = null;
          Action(nd);
        }
      }
    }
  }

  void MoveNocca(int[] node, int turn)
  {
    if (node[0] == 0 && node[1] == 0) return;
    int height1 = 0;
    int height2 = 0;
    if (_map[node[0], 2] != 0)
    {
      height1 = 3;
      _map[node[0], 2] = 0;
    }
    else if (_map[node[0], 1] != 0)
    {
      height1 = 2;
      _map[node[0], 1] = 0;
    }
    else if (_map[node[0], 0] != 0)
    {
      height1 = 1;
      _map[node[0], 0] = 0;
    }


    if (_map[node[1], 1] != 0)
    {
      height2 = 2;
      _map[node[1], 2] = turn;
    }
    else if (_map[node[1], 0] != 0)
    {
      height2 = 1;
      _map[node[1], 1] = turn;
    }
    else
    {
      _map[node[1], 0] = turn;
    }
    SyncPosition();
    // ゴールしてたらゲーム終了
    if(turn == 1 && node[1] < 5){
      winner = 1;
    }else if(turn == -1 && node[1] > 34){
      winner = -1;
    }
  }


  public void Reset()
  {
    _map = new int[,]{
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {-1,0,0},{-1,0,0},{-1,0,0},{-1,0,0},{-1,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
      {1,0,0},{1,0,0},{1,0,0},{1,0,0},{1,0,0},
      {0,0,0},{0,0,0},{0,0,0},{0,0,0},{0,0,0},
    };
    moved = 0;
    winner = 0;
    SyncPosition();
    Text text = messageText.GetComponent<Text>();
    text.text = "";
  }

  private void SyncPosition()
  {
    int num = 1, num_e = 1;
    // mapを検索して駒の位置を調整
    for (int i = 0; i < _map.GetLength(0); i++)
    {
      for (int j = 0; j < _map.GetLength(1); j++)
      {
        if(_map[i, j] != 0){
          var nc = (_map[i, j] == 1) ? GameObject.Find("Nocca" + num++) : GameObject.Find("Nocca_E" + num_e++);
          nc.transform.position = new Vector3(-i % 5, 0.8f + j*0.8f, (i/5));
        }
      }
    }
  }


  public void Pass()
  {
    var (enemyhand, enemyScore) = enemy.GetComponent<EnemyAI>().thinkHand(_map, moved);
    MoveNocca(enemyhand, -1);
  }

  void Action(int[] nd)
  {
    if (!CanPut(_map, nd, 1)) return;
    moved += 1;
    MoveNocca(nd, 1);
    isEnemyThinking = true;
  }

  void EnemyAction()
  {
    var (enemyhand, enemyScore) = enemy.GetComponent<EnemyAI>().thinkHand(_map, moved);
    UnityEngine.Debug.Log(moved);
    UnityEngine.Debug.Log("hand: " + enemyhand[0]+enemyhand[1] + ", sc: " + enemyScore);
    MoveNocca(enemyhand, -1);
  }

  Boolean CanPut(int[,] map, int[] node, int turn)
  {
    int p1 = node[0];
    int p2 = node[1];
    // 移動元：自分の駒が上にあるか判定
    if (map[p1, 2] == -turn) return false;
    if (map[p1, 2] == 0 && map[p1, 1] == -turn) return false;
    if (map[p1, 0] != turn && map[p1, 1] != turn && map[p1, 2] != turn) return false;
    // 移動先：周囲8マスにあるかどうか判定
    int row = p1 / 5;
    int column = p1 % 5;
    int row_to = p2 / 5;
    int column_to = p2 % 5;
    if (row_to < row - 1 || row + 1 < row_to) return false;
    if (column_to < column - 1 || column + 1 < column_to) return false;
    if (p1 == p2) return false;
    // ３個乗っていないか判定
    if (map[p2, 2] != 0) return false;
    // 自分の陣地かどうか判定
    if ((turn == 1 && p2 > 34) || (turn == -1 && p2 < 5)) return false;
    return true;
  }

}
