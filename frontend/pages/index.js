import Head from 'next/head';

export default function Home() {
  return (
    <div>
      <Head>
        <title>Flow Studio</title>
        <meta name="description" content="The visual builder for the Flow framework" />
        <link rel="icon" href="/favicon.ico" />
      </Head>

      <main style={{ fontFamily: 'sans-serif', textAlign: 'center', marginTop: '5rem' }}>
        <h1>
          Welcome to <a href="https://github.com/The-Pocket/Flow" style={{ textDecoration: 'none', color: '#0070f3' }}>Flow Studio</a>!
        </h1>
        <p>The visual builder for the Flow framework.</p>
      </main>
    </div>
  );
}
