import Link from "next/link";

export default function Blog() {
    return (
        <div>
        <h1>Blog</h1>
        <p>Blog page content goes here.</p>
        <p>
            <Link href="/blog/first-post">First Post</Link>
        </p>
        <p>
            <Link href="/blog/second-post">Second Post</Link>
        </p>
        </div>
    );
}