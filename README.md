### API Документация

**Базовый путь:** `/api`

#### Посты

| Метод                                    | Путь                  | Параметры запроса                        | Описание            | Ответ                     | Код ответа            |
| ---------------------------------------- | --------------------- | ---------------------------------------- | ------------------- | ------------------------- | --------------------- |
| GET                                      | `/post`               | `limit` (int, query, опц., по умолч. 10) |                     |                           |                       |
| `offset` (int, query, опц., по умолч. 0) | Получить ленту постов | `PostPreviewDto[]`                       | 200 OK              |                           |                       |
| GET                                      | `/post/{id}`          | —                                        | Получить пост по ID | `Post` либо 404 Not Found | 200 OK, 404 Not Found |

##### Схемы ответов

**PostPreviewDto**

```json
{
  "id": long,
  "text": string,
  "channelName": string,
  "channelAvatar": string,
  "commentsCount": int,
  "createdAt": string (ISO 8601 UTC),
  "imageUrls": [ string ]
}
```

**Post**

```json
{
  "id": long,
  "postId": int,
  "mediaGroup": long|null,
  "inChatId": int,
  "text": string|null,
  "imagesFileId": [ string ]|null,
  "isDelete": bool,
  "isEdit": bool,
  "messageReactionCount": int,
  "channelId": long,
  "createdAt": string (ISO 8601 UTC)
}
```

---

#### Комментарии

| Метод                          | Путь                           | Параметры запроса             | Описание | Ответ | Код ответа |
| ------------------------------ | ------------------------------ | ----------------------------- | -------- | ----- | ---------- |
| GET                            | `/comment`                     | `postId` (long, query, обяз.) |          |       |            |
| `limit` (int, query, опц., 10) |                                |                               |          |       |            |
| `offset` (int, query, опц., 0) | Получить комментарии к посту   | `CommentDto[]`                | 200 OK   |       |            |
| GET                            | `/comment/{commentId}/replies` | `postId` (long, query, обяз.) |          |       |            |
| `limit` (int, query, опц., 10) |                                |                               |          |       |            |
| `offset` (int, query, опц., 0) | Получить ответы на комментарий | `CommentDto[]`                | 200 OK   |       |            |

##### Схема ответа

**CommentDto**

```json
{
  "id": long,
  "postId": long,
  "parentCommentId": long|null,
  "author": string,
  "text": string,
  "createdAt": string (ISO 8601 UTC)
}
```
