import { createContext, useState, useContext, Dispatch, SetStateAction, ReactNode } from "react";

interface Data {
    pokemonName: string;
    cardNumber: string;
    formatNumber:string;
}

// Create the context with a default type
export const ImageResponseContext = createContext<{
    responseData: Data | null;
    setResponseData: Dispatch<SetStateAction<Data | null>>;
}>({
    responseData: null,
    setResponseData: () => {},
});

export const ImageResponseProvider = ({ children }: { children: ReactNode }) => {
 const [responseData, setResponseData] = useState<Data | null>(null);

 // Directly use setResponseData to update both pokemonName and cardNumber
 const updateResponseData = (newData: Data | null) => {
    setResponseData(newData);
 };

 return (
      <ImageResponseContext.Provider value={{ responseData,setResponseData }}>
          {children}
      </ImageResponseContext.Provider>
 );
};
