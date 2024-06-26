"use client";

import { useRouter, useSearchParams } from "next/navigation";
import { use, useEffect, useState } from "react";

type ResponseAuth = {
  access_token: string;
  token_type: string;
  refresh_token: string;
  expires_in: number;
};

const Page = () => {
  const [redirectparams, setUrl] = useState<ResponseAuth | null>();
  const searchParams = useSearchParams();
  const router = useRouter();

  useEffect(() => {
    // console.log(searchParams.toString());
    if (searchParams) {
      fetch(`https://localhost:7289/api/auth/callback?${searchParams.toString()}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
        },
      }).then((res) =>
        res.json().then((json) => {
          console.log(json);
        })
      );
    }
  }, []);

  //   useEffect(() => {
  //     if (router.isReady())
  // }, [])

  return <div>Callback</div>;
};

export default Page;
