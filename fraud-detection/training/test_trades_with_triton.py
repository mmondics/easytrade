import requests
import json

# Triton inference endpoint
# TRITON_URL = "http://<your_hostname>/v2/models/fraud_ensemble/infer"

# Example test cases with varying levels of suspicion
test_cases = [
    {
        "desc": "Normal buy, small value",
        "data": [4.0, 1.0, 0.0, 10.0, 25.0, 250.0, 14.0, 2.0, 1.0]
    },
    {
        "desc": "Large value trade",
        "data": [6.0, 2.0, 1.0, 150.0, 100.0, 15000.0, 10.0, 1.0, 1.0]
    },
    {
        "desc": "Failed trade at night",
        "data": [8.0, 3.0, 0.0, 25.0, 300.0, 7500.0, 2.0, 5.0, 0.0]
    },
    {
        "desc": "Successful trade at night",
        "data": [5.0, 4.0, 1.0, 50.0, 50.0, 2500.0, 23.0, 4.0, 1.0]
    },
    {
        "desc": "Trade with high quantity but low value",
        "data": [7.0, 5.0, 0.0, 10000.0, 0.2, 2000.0, 12.0, 0.0, 1.0]
    },
    {
        "desc": "High value failed trade",
        "data": [6.0, 2.0, 0.0, 100.0, 200.0, 20000.0, 13.0, 3.0, 0.0]
    }
]

# Function to send inference request
def send_inference(trade_data):
    payload = {
        "inputs": [
            {
                "name": "raw_input",
                "shape": [1,9],
                "datatype": "FP32",
                "data": trade_data
            }
        ]
    }
    response = requests.post(TRITON_URL, headers={"Content-Type": "application/json"}, json=payload)
    return response.json()

# Run all test cases
for i, test in enumerate(test_cases):
    result = send_inference(test["data"])
    try:
        score = result["outputs"][0]["data"][0]
    except Exception as e:
        score = f"Error: {e}"
    print(f"Test {i+1}: {test['desc']}")
    print(f"  Input: {test['data']}")
    print(f"  Fraud Score: {score}\n")
