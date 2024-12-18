using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRandomMovement : MonoBehaviour
{
    public GameObject[] fishes; // ������ ������Ʈ �迭
    public Transform leftWall; // ���� WALL
    public Transform rightWall; // ������ WALL
    public float speedMin = 1f; // �ּ� �ӵ�
    public float speedMax = 5f; // �ִ� �ӵ�

    private List<GameObject> movingFishes = new List<GameObject>(); // ���� Moving ������ ������
    private const int MaxMovingFishes = 4; // �ִ� Moving ������ ����� ��

    void Start()
    {
        // �ʱ� Moving ���¸� ������ 4�� ����⿡ ����
        ActivateRandomFishes(MaxMovingFishes);
    }

    void Update()
    {
        for (int i = movingFishes.Count - 1; i >= 0; i--)
        {
            GameObject fish = movingFishes[i];
            if (fish != null)
            {
                MoveFish(fish);
            }
        }

        // ��� ����� �������� ���� �� �� ����� Ȱ��ȭ
        while (movingFishes.Count < MaxMovingFishes)
        {
            ActivateRandomFishes(1);
        }
    }


    void MoveFish(GameObject fish)
    {
        FishController controller = fish.GetComponent<FishController>();
        if (controller != null && controller.IsMoving)
        {
            float speed = controller.Speed * Time.deltaTime;
            Vector3 moveDirection = controller.IsFlipped ? Vector3.right : Vector3.left;
            fish.transform.Translate(moveDirection * speed);

            // �� �浹 üũ
            if (!controller.IsFlipped && fish.transform.position.x <= leftWall.position.x)
            {
                StopAndFlip(fish);
            }
            else if (controller.IsFlipped && fish.transform.position.x >= rightWall.position.x)
            {
                StopAndFlip(fish);
            }
        }
    }

    void StopAndFlip(GameObject fish)
    {
        FishController controller = fish.GetComponent<FishController>();
        if (controller != null)
        {
            controller.IsMoving = false;
            controller.IsFlipped = !controller.IsFlipped; // ���� ����
            movingFishes.Remove(fish); // ����Ʈ���� ����
        }
    }

    void ActivateRandomFishes(int count)
    {
        if (fishes == null || fishes.Length == 0)
        {
            Debug.LogError("Fishes array is not assigned!");
            return;
        }

        List<GameObject> availableFishes = new List<GameObject>(fishes);
        availableFishes.RemoveAll(f => movingFishes.Contains(f));

        for (int i = 0; i < count && availableFishes.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableFishes.Count);
            GameObject selectedFish = availableFishes[randomIndex];
            availableFishes.RemoveAt(randomIndex);

            FishController controller = selectedFish.GetComponent<FishController>();
            if (controller != null)
            {
                controller.Speed = Random.Range(speedMin + controller.MinPlusSpeed, speedMax + controller.MaxPlusSpeed);
                controller.IsMoving = true;
                movingFishes.Add(selectedFish);
            }
        }
    }
}


