# Домашнє завдання

---

## 2й рівень складності

Реалізувати функціонал для Owner, який дозволяє:
- отримати `guestSessionId` власної owner session для конкретного reservation;
- підтвердити консолідований груповий pre-order.

### 1. Отримати owner session id

**GET /reservations/{reservationId}/guest-session-id**

**Response:**

```json
{
  "guestSessionId": "owner-session-123"
}
```

### 2. Підтвердити консолідований pre-order

**POST /group-reservations/{groupReservationId}/submit**

**Request:**

```json
{
  "reservationId": "reservation-123"
}
```

**Опис:**  
Після перегляду замовлень усіх учасників Owner має підтвердити консолідований pre-order, який буде переданий ресторану для подальшої обробки.

Під час submit потрібно:
- взяти лише `submitted` guest sessions;
- зібрати всі страви в один список;
- об’єднати однакові страви по `dishId`;
- додати їх в order;
- змінити статус order на `SubmittedByCustomer`;
- змінити статус group reservation на `Submitted`.

**Response:**

```json
[
  {
    "dishId": "dish1",
    "quantity": 3
  },
  {
    "dishId": "dish2",
    "quantity": 2
  }
]
```

**Умови:**
- тільки Owner може викликати обидва endpoint-и;
- якщо reservation або group reservation не знайдено — повернути 404;
- якщо немає жодної `submitted` session — повернути 400;
- якщо в submitted sessions немає жодної страви — повернути 400;
- якщо order не знайдено — повернути 400.
