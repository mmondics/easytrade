import numpy as np
import joblib
import triton_python_backend_utils as pb_utils

class TritonPythonModel:
    def initialize(self, args):
        # Load the StandardScaler
        self.scaler = joblib.load("/models/fraud_triton_model_repo/fraud_preprocessor/1/trade_input_scaler.pkl")

    def execute(self, requests):
        responses = []
        for request in requests:
            # Get the raw input tensor (expects [batch_size, 9])
            input_tensor = pb_utils.get_input_tensor_by_name(request, "raw_input")
            input_data = input_tensor.as_numpy()  # Do NOT reshape!

            # Ensure float32 dtype for consistency
            input_data = input_data.astype(np.float32)

            # Apply standard scaling
            scaled_data = self.scaler.transform(input_data)

            # Create the output tensor with name "input" for next step in ensemble
            output_tensor = pb_utils.Tensor("input", scaled_data.astype(np.float32))

            # Package and return the response
            inference_response = pb_utils.InferenceResponse(output_tensors=[output_tensor])
            responses.append(inference_response)

        return responses
