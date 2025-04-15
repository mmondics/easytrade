
import pandas as pd
import numpy as np
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import StandardScaler
from sklearn.metrics import classification_report, confusion_matrix
import torch
import torch.nn as nn
import torch.onnx
import matplotlib.pyplot as plt
import seaborn as sns
from imblearn.over_sampling import SMOTE

# Load data
df = pd.read_csv('trade_data.csv')

print("Original class distribution:")
print(df['Suspicious'].value_counts())

# Features and label
X = df.drop(columns=['Suspicious'])
y = df['Suspicious']

# Normalize features
scaler = StandardScaler()
X_scaled = scaler.fit_transform(X)

# Apply SMOTE to balance classes
smote = SMOTE(random_state=42)
X_resampled, y_resampled = smote.fit_resample(X_scaled, y)

print("\nResampled class distribution:")
print(pd.Series(y_resampled).value_counts())

# Train/test split (on resampled data)
X_train, X_test, y_train, y_test = train_test_split(
    X_resampled, y_resampled, test_size=0.2, random_state=42, stratify=y_resampled
)

# Convert to tensors
X_train_tensor = torch.tensor(X_train, dtype=torch.float32)
y_train_tensor = torch.tensor(y_train.values, dtype=torch.float32).unsqueeze(1)
X_test_tensor = torch.tensor(X_test, dtype=torch.float32)
y_test_tensor = torch.tensor(y_test.values, dtype=torch.float32).unsqueeze(1)

# Define model with dropout
class FraudNetGeneralized(nn.Module):
    def __init__(self, input_dim):
        super(FraudNetGeneralized, self).__init__()
        self.net = nn.Sequential(
            nn.Linear(input_dim, 64),
            nn.ReLU(),
            nn.Dropout(0.2),
            nn.Linear(64, 32),
            nn.ReLU(),
            nn.Dropout(0.2),
            nn.Linear(32, 1),
            nn.Sigmoid()
        )

    def forward(self, x):
        return self.net(x)

model = FraudNetGeneralized(X_train.shape[1])
criterion = nn.BCELoss()
optimizer = torch.optim.Adam(model.parameters(), lr=0.001)

# Early stopping setup
epochs = 150
best_val_loss = float('inf')
patience = 10
trigger = 0

# Training loop with early stopping
for epoch in range(epochs):
    model.train()
    optimizer.zero_grad()
    output = model(X_train_tensor)
    loss = criterion(output, y_train_tensor)
    loss.backward()
    optimizer.step()

    # Validation loss
    model.eval()
    with torch.no_grad():
        val_preds = model(X_test_tensor)
        val_loss = criterion(val_preds, y_test_tensor).item()

    print(f"Epoch {epoch+1}/{epochs}, Train Loss: {loss.item():.4f}, Val Loss: {val_loss:.4f}")

    if val_loss < best_val_loss:
        best_val_loss = val_loss
        trigger = 0
    else:
        trigger += 1
        if trigger >= patience:
            print(f"Early stopping triggered at epoch {epoch+1}")
            break

# Evaluation
model.eval()
with torch.no_grad():
    y_pred_probs = model(X_test_tensor).squeeze().numpy()
    y_pred = (y_pred_probs > 0.5).astype(int)

# from sklearn.metrics import precision_recall_curve
# import matplotlib.pyplot as plt

# precision, recall, thresholds = precision_recall_curve(y_test, y_pred_probs)

# plt.plot(thresholds, precision[:-1], label='Precision')
# plt.plot(thresholds, recall[:-1], label='Recall')
# plt.xlabel('Threshold')
# plt.ylabel('Score')
# plt.title('Precision vs. Recall at Different Thresholds')
# plt.legend()
# plt.grid(True)
# plt.show()

print("\nClassification Report:")
print(classification_report(y_test, y_pred, digits=4))

# Confusion matrix
cm = confusion_matrix(y_test, y_pred)
sns.heatmap(cm, annot=True, fmt='d', cmap='Blues', 
            xticklabels=['Not Suspicious', 'Suspicious'], 
            yticklabels=['Not Suspicious', 'Suspicious'])
plt.xlabel('Predicted')
plt.ylabel('True')
plt.title('Confusion Matrix')
plt.tight_layout()
plt.savefig('confusion_matrix.png')
plt.close()
print("\nConfusion Matrix:")
print(cm)

# Export model
dummy_input = torch.randn(1, X_train.shape[1])
torch.onnx.export(
    model,
    dummy_input,
    "model.onnx",
    input_names=["input"],
    output_names=["output"],
    dynamic_axes={"input": {0: "batch_size"}, "output": {0: "batch_size"}},
    opset_version=11
)

print("✅ Model exported to model.onnx")