# Домашнє завдання

---

## 1й рівень складності

Реалізувати API для отримання списку сесій учасників групового pre-order для Owner

**GET /guest/groups/{groupReservationId}/sessions**

**Опис:**  
Owner має бачити, які учасники приєдналися до групового замовлення, які страви вони додали, загальну суму по кожному учаснику та загальну суму по всіх учасниках.

**Response:**

```json
{
  "participants": [
    {
      "id": "session1",
      "type": "Guest",
      "name": "Andrii",
      "items": [
        {
          "dish": {
            "id": "dish1",
            "name": "Caesar Salad",
            "price": 180
          },
          "quantity": 2
        }
      ],
      "total": 360,
      "isSubmitted": true
    },
    {
      "id": "session2",
      "type": "Guest",
      "name": "Olena",
      "items": [
        {
          "dish": {
            "id": "dish2",
            "name": "Tom Yum",
            "price": 220
          },
          "quantity": 1
        }
      ],
      "total": 220,
      "isSubmitted": false
    }
  ],
  "total": 580
}
```

