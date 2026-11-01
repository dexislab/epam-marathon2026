# Домашнє завдання

---

## 3й рівень складності

Розширити функціонал керування груповим pre-order для Owner:
- реалізувати видалення guest session із групи;
- після видалення повертати оновлений список учасників і нову загальну суму.

**DELETE /guest/groups/{groupReservationId}/sessions/{guestSessionId}**

**Опис:**  
Owner може видалити сесію конкретного учасника з групового замовлення.

**Response:**

```json
{
  "participants": [
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
  "total": 220
}
```

**Умови:**
- тільки Owner може видаляти guest session;
- якщо guest session не належить цій group reservation — повернути 400;
- якщо guest session не знайдена — повернути 404;
- після видалення потрібно повернути оновлений snapshot групи.
