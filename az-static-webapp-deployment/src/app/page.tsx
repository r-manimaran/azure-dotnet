import Image from "next/image";
async function getData() {
  // Add a small delay to simulate loading state
  const res = await fetch('https://randomuser.me/api/',{
    // Disable cache to get fresh data on each request
    cache: 'no-store'
  });

  if(!res.ok) {
    throw new Error('Failed to fetch data');  
  }
  
  return res.json();

}

export default async function Home() {
  // Fetch the data
  const data = await getData();
  return (
    <div className="grid grid-rows-[20px_1fr_20px] items-center justify-items-center min-h-screen p-8 pb-20 gap-16 sm:p-20 font-[family-name:var(--font-geist-sans)]">
      <main className="flex flex-col gap-[32px] row-start-2 items-center sm:items-start">
        <Image
          className="dark:invert"
          src="/next.svg"
          alt="Next.js logo"
          width={180}
          height={38}
          priority
        />
        
        {/* User Information Card */}
        <div className="w-full max-w-md bg-white shadow-lg rounded-lg overflow-hidden p-6 dark:bg-gray-800">
          <h2 className="text-2xl font-bold mb-4 text-gray-800 dark:text-white">
            User Information
          </h2>
          
          <div className="space-y-4">
            {/* Email */}
            <div className="flex flex-col">
              <label className="text-sm font-medium text-gray-600 dark:text-gray-300">
                Email
              </label>
              <p className="text-gray-800 dark:text-white">
                {data.results[0].email}
              </p>
            </div>

            {/* Phone */}
            <div className="flex flex-col">
              <label className="text-sm font-medium text-gray-600 dark:text-gray-300">
                Phone
              </label>
              <p className="text-gray-800 dark:text-white">
                {data.results[0].phone}
              </p>
            </div>

            {/* Name */}
            <div className="flex flex-col">
              <label className="text-sm font-medium text-gray-600 dark:text-gray-300">
                Name
              </label>
              <p className="text-gray-800 dark:text-white">
                {data.results[0].name.first} {data.results[0].name.last}
              </p>
            </div>
          </div>
        </div>

        {/* Refresh Button */}
        <form action="/refresh" className="mt-4">
          <button 
            className="bg-blue-500 hover:bg-blue-600 text-white font-bold py-2 px-4 rounded transition-colors"
            type="submit"
          >
            Refresh Data
          </button>
        </form>
      </main>
    </div>
  );
}
