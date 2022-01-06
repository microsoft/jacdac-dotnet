namespace Jacdac {
    public static partial class ServiceClasses
    {
        public const uint ModelRunner = 0x140f9a78;
    }

    public enum ModelRunnerModelFormat: uint { // uint32_t
        TFLite = 0x334c4654,
        ML4F = 0x30470f62,
        EdgeImpulseCompiled = 0x30564945,
    }

    public enum ModelRunnerCmd {
        /// <summary>
        /// Argument: model_size B uint32_t. Open pipe for streaming in the model. The size of the model has to be declared upfront.
        /// The model is streamed over regular pipe data packets.
        /// The format supported by this instance of the service is specified in `format` register.
        /// When the pipe is closed, the model is written all into flash, and the device running the service may reset.
        ///
        /// ```
        /// const [modelSize] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        SetModel = 0x80,

        /// <summary>
        /// report SetModel
        /// ```
        /// const [modelPort] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>

        /// <summary>
        /// Argument: outputs pipe (bytes). Open channel that can be used to manually invoke the model. When enough data is sent over the `inputs` pipe, the model is invoked,
        /// and results are send over the `outputs` pipe.
        ///
        /// ```
        /// const [outputs] = jdunpack<[Uint8Array]>(buf, "b[12]")
        /// ```
        /// </summary>
        Predict = 0x81,

        /// <summary>
        /// report Predict
        /// ```
        /// const [inputs] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
    }

    public static class ModelRunnerCmdPack {
        /// <summary>
        /// Pack format for 'set_model' register data.
        /// </summary>
        public const string SetModel = "u32";

        /// <summary>
        /// Pack format for 'set_model' register data.
        /// </summary>
        public const string SetModelReport = "u16";

        /// <summary>
        /// Pack format for 'predict' register data.
        /// </summary>
        public const string Predict = "b[12]";

        /// <summary>
        /// Pack format for 'predict' register data.
        /// </summary>
        public const string PredictReport = "u16";
    }

    public enum ModelRunnerReg {
        /// <summary>
        /// Read-write uint16_t. When register contains `N > 0`, run the model automatically every time new `N` samples are collected.
        /// Model may be run less often if it takes longer to run than `N * sampling_interval`.
        /// The `outputs` register will stream its value after each run.
        /// This register is not stored in flash.
        ///
        /// ```
        /// const [autoInvokeEvery] = jdunpack<[number]>(buf, "u16")
        /// ```
        /// </summary>
        AutoInvokeEvery = 0x80,

        /// <summary>
        /// Read-only. Results of last model invocation as `float32` array.
        ///
        /// ```
        /// const [output] = jdunpack<[number[]]>(buf, "f32[]")
        /// ```
        /// </summary>
        Outputs = 0x101,

        /// <summary>
        /// Read-only. The shape of the input tensor.
        ///
        /// ```
        /// const [dimension] = jdunpack<[number[]]>(buf, "u16[]")
        /// ```
        /// </summary>
        InputShape = 0x180,

        /// <summary>
        /// Read-only. The shape of the output tensor.
        ///
        /// ```
        /// const [dimension] = jdunpack<[number[]]>(buf, "u16[]")
        /// ```
        /// </summary>
        OutputShape = 0x181,

        /// <summary>
        /// Read-only μs uint32_t. The time consumed in last model execution.
        ///
        /// ```
        /// const [lastRunTime] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        LastRunTime = 0x182,

        /// <summary>
        /// Read-only B uint32_t. Number of RAM bytes allocated for model execution.
        ///
        /// ```
        /// const [allocatedArenaSize] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        AllocatedArenaSize = 0x183,

        /// <summary>
        /// Read-only B uint32_t. The size of the model in bytes.
        ///
        /// ```
        /// const [modelSize] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        ModelSize = 0x184,

        /// <summary>
        /// Read-only string (bytes). Textual description of last error when running or loading model (if any).
        ///
        /// ```
        /// const [lastError] = jdunpack<[string]>(buf, "s")
        /// ```
        /// </summary>
        LastError = 0x185,

        /// <summary>
        /// Constant ModelFormat (uint32_t). The type of ML models supported by this service.
        /// `TFLite` is flatbuffer `.tflite` file.
        /// `ML4F` is compiled machine code model for Cortex-M4F.
        /// The format is typically present as first or second little endian word of model file.
        ///
        /// ```
        /// const [format] = jdunpack<[ModelRunnerModelFormat]>(buf, "u32")
        /// ```
        /// </summary>
        Format = 0x186,

        /// <summary>
        /// Constant uint32_t. A version number for the format.
        ///
        /// ```
        /// const [formatVersion] = jdunpack<[number]>(buf, "u32")
        /// ```
        /// </summary>
        FormatVersion = 0x187,

        /// <summary>
        /// Constant bool (uint8_t). If present and true this service can run models independently of other
        /// instances of this service on the device.
        ///
        /// ```
        /// const [parallel] = jdunpack<[number]>(buf, "u8")
        /// ```
        /// </summary>
        Parallel = 0x188,
    }

    public static class ModelRunnerRegPack {
        /// <summary>
        /// Pack format for 'auto_invoke_every' register data.
        /// </summary>
        public const string AutoInvokeEvery = "u16";

        /// <summary>
        /// Pack format for 'outputs' register data.
        /// </summary>
        public const string Outputs = "r: f32";

        /// <summary>
        /// Pack format for 'input_shape' register data.
        /// </summary>
        public const string InputShape = "r: u16";

        /// <summary>
        /// Pack format for 'output_shape' register data.
        /// </summary>
        public const string OutputShape = "r: u16";

        /// <summary>
        /// Pack format for 'last_run_time' register data.
        /// </summary>
        public const string LastRunTime = "u32";

        /// <summary>
        /// Pack format for 'allocated_arena_size' register data.
        /// </summary>
        public const string AllocatedArenaSize = "u32";

        /// <summary>
        /// Pack format for 'model_size' register data.
        /// </summary>
        public const string ModelSize = "u32";

        /// <summary>
        /// Pack format for 'last_error' register data.
        /// </summary>
        public const string LastError = "s";

        /// <summary>
        /// Pack format for 'format' register data.
        /// </summary>
        public const string Format = "u32";

        /// <summary>
        /// Pack format for 'format_version' register data.
        /// </summary>
        public const string FormatVersion = "u32";

        /// <summary>
        /// Pack format for 'parallel' register data.
        /// </summary>
        public const string Parallel = "u8";
    }

}
