#!/bin/bash

API_URL="http://localhost:5000/api-conversation/stream"
CONVERSATION_ID="cli-chat"

clear

echo "======================================="
echo "         RagDemo CLI Chat"
echo "======================================="
echo
echo "Conversation: $CONVERSATION_ID"
echo
echo "Type 'exit' to quit."
echo

while true
do
    read -rp "You: " QUESTION

    if [[ "$QUESTION" == "exit" ]]; then
        break
    fi

    echo
    echo "Thinking..."
    echo

    RESPONSE=$(curl -s "$API_URL" \
      --request POST \
      --header "Content-Type: application/json" \
      --data "{
        \"conversationId\":\"$CONVERSATION_ID\",
        \"question\":\"$QUESTION\"
      }")

    echo "Assistant:"
    echo "---------------------------------------"

    ANSWER=$(echo "$RESPONSE" |
      sed -n '/event: completed/q;p' |
      grep -v '^event:' |
      grep -v '^data:')

    echo "$ANSWER"
    echo

    RETRIEVAL=$(echo "$RESPONSE" |
      awk '
      /event: retrieval/ {getline; print}
      ')

    COMPLETED=$(echo "$RESPONSE" |
      awk '
      /event: completed/ {getline; print}
      ')

    echo "Diagnostics"
    echo "---------------------------------------"
    echo "$RETRIEVAL"
    echo "$COMPLETED"
    echo
done