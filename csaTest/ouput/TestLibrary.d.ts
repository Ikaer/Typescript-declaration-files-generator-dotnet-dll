declare namespace TestLibrary {
	interface Class0 {
		PropX?: boolean | string
		PropY?: Class0
	}

	interface Class1 {
		MyProp_DateTime: string
		MyProp_Guid?: string
		MyProp_Int: number
		MyProp_String: string
		/**
		* any: because System.TimeSpan has been exclude
		*/
		MyProp_Timespan: any
		AnotherProp?: Class0
	}

}

